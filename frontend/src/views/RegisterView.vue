<script lang="ts" setup>
  import { toTypedSchema } from "@vee-validate/zod";
  import { z } from "zod";
  import { useForm } from "vee-validate";
  import { ref } from "vue";
  import { useRouter, RouterLink } from "vue-router";
  import { useAuthStore } from "@/stores/auth";

  const BACKEND_URL = import.meta.env.VITE_BACKEND_URL;
  const router = useRouter();
  const authStore = useAuthStore();
  const serverError = ref<string | null>(null);
  const isLoading = ref(false);

  const schema = toTypedSchema(
    z
      .object({
        name: z
          .string({ message: "El nombre es obligatorio" })
          .min(3, { message: "El nombre debe tener al menos 3 caracteres" })
          .max(100, { message: "El nombre no debe superar los 100 caracteres" }),
        email: z
          .string({ message: "El correo es obligatorio" })
          .email({ message: "El correo es inválido." }),
        password: z
          .string({ message: "La contraseña es obligatoria" })
          .min(8, { message: "La contraseña debe tener mínimo 8 caracteres" }),
        confirmPassword: z
          .string({ message: "Debes confirmar la contraseña" })
          .min(8, { message: "La contraseña debe tener mínimo 8 caracteres" }),
      })
      .refine((data) => data.password === data.confirmPassword, {
        message: "Las contraseñas no coinciden",
        path: ["confirmPassword"],
      }),
  );

  const { errors, handleSubmit, defineField } = useForm({
    validationSchema: schema,
  });

  const [name, nameAttrs] = defineField("name");
  const [email, emailAttrs] = defineField("email");
  const [password, passwordAttrs] = defineField("password");
  const [confirmPassword, confirmPasswordAttrs] = defineField("confirmPassword");

  const onSubmit = handleSubmit(async (values) => {
    try {
      serverError.value = null;
      isLoading.value = true;
      const request = await fetch(`${BACKEND_URL}/v1/auth/register`, {
        headers: {
          "Content-Type": "application/json;charset=utf-8",
        },
        body: JSON.stringify({
          name: values.name,
          email: values.email,
          password: values.password,
          confirmPassword: values.confirmPassword,
        }),
        method: "POST",
      });

      if (!request.ok) {
        let errorMsg = "No se pudo completar el registro";
        try {
          const err = await request.json();
          errorMsg = err?.message ?? errorMsg;
        } catch {
          errorMsg = "Error en el servidor. Verifica que la base de datos esté activa.";
        }
        serverError.value = errorMsg;
        return;
      }

      const response = await request.json();
      console.log(response);
      authStore.setUser({
        name: response.name,
        email: response.email,
        token: response.token,
      });
      router.push("/");
    } catch (error) {
      console.log(error);
      serverError.value = "Error de conexión con el servidor";
    } finally {
      isLoading.value = false;
    }
  });
</script>

<template>
  <div class="auth-wrapper">
    <div class="auth-card">
      <h1 class="auth-title">Register</h1>
      <p class="auth-subtitle">Crea tu cuenta para comenzar</p>

      <form class="auth-form" @submit.prevent="onSubmit">
        <fieldset class="auth-fieldset">
          <label class="auth-label" for="reg-name">Nombre</label>
          <input
            id="reg-name"
            class="auth-input"
            type="text"
            placeholder="Tu nombre completo"
            v-model="name"
            v-bind="nameAttrs"
            :class="{ 'auth-input--error': errors.name }"
          />
          <span class="auth-error-msg">{{ errors.name }}</span>
        </fieldset>

        <fieldset class="auth-fieldset">
          <label class="auth-label" for="reg-email">Email</label>
          <input
            id="reg-email"
            class="auth-input"
            type="text"
            placeholder="tucorreo@ejemplo.com"
            v-model="email"
            v-bind="emailAttrs"
            :class="{ 'auth-input--error': errors.email }"
          />
          <span class="auth-error-msg">{{ errors.email }}</span>
        </fieldset>

        <fieldset class="auth-fieldset">
          <label class="auth-label" for="reg-password">Contraseña</label>
          <input
            id="reg-password"
            class="auth-input"
            type="password"
            placeholder="Mínimo 8 caracteres"
            v-model="password"
            v-bind="passwordAttrs"
            :class="{ 'auth-input--error': errors.password }"
          />
          <span class="auth-error-msg">{{ errors.password }}</span>
        </fieldset>

        <fieldset class="auth-fieldset">
          <label class="auth-label" for="reg-confirm">Confirmar Contraseña</label>
          <input
            id="reg-confirm"
            class="auth-input"
            type="password"
            placeholder="Repite tu contraseña"
            v-model="confirmPassword"
            v-bind="confirmPasswordAttrs"
            :class="{ 'auth-input--error': errors.confirmPassword }"
          />
          <span class="auth-error-msg">{{ errors.confirmPassword }}</span>
        </fieldset>

        <span v-if="serverError" class="auth-server-error">{{ serverError }}</span>

        <button class="auth-btn" type="submit" :disabled="isLoading">
          {{ isLoading ? "Registrando..." : "Crear cuenta" }}
        </button>
      </form>

      <div class="auth-links">
        <RouterLink to="/login" class="auth-link">
          ¿Ya tienes cuenta? <span class="auth-link--accent">Inicia sesión</span>
        </RouterLink>
      </div>
    </div>
  </div>
</template>

<style lang="css" scoped>
  .auth-wrapper {
    min-height: 100vh;
    width: 100%;
    display: flex;
    align-items: center;
    justify-content: center;
    background-color: #f4f5f7;
    padding: 1.5rem;
    margin-top: calc(-1 * var(--marginBody, 3.125rem));
  }

  .auth-card {
    background-color: #ffffff;
    border: 1px solid #e2e8f0;
    border-radius: 8px;
    box-shadow: 0 1px 4px rgba(0, 0, 0, 0.06);
    padding: 2.5rem 2rem;
    width: 100%;
    max-width: 420px;
    display: flex;
    flex-direction: column;
    gap: 1.25rem;
  }

  .auth-title {
    font-size: 1.6rem;
    font-weight: 600;
    color: #0f172a;
    margin: 0;
  }

  .auth-subtitle {
    font-size: 0.875rem;
    color: #64748b;
    margin-top: -0.75rem;
  }

  .auth-form {
    display: flex;
    flex-direction: column;
    gap: 1rem;
  }

  .auth-fieldset {
    border: none;
    padding: 0;
    display: flex;
    flex-direction: column;
    gap: 0.35rem;
  }

  .auth-label {
    font-size: 0.8125rem;
    font-weight: 500;
    color: #374151;
  }

  .auth-input {
    padding: 0.6rem 0.875rem;
    font-size: 0.9375rem;
    font-family: inherit;
    border: 1px solid #cbd5e1;
    border-radius: 6px;
    color: #0f172a;
    background-color: #ffffff;
    outline: none;
    transition: border-color 0.15s ease, box-shadow 0.15s ease;
  }

  .auth-input::placeholder {
    color: #94a3b8;
  }

  .auth-input:focus {
    border-color: #0f766e;
    box-shadow: 0 0 0 3px rgba(15, 118, 110, 0.1);
  }

  .auth-input--error {
    border-color: #dc2626;
  }

  .auth-input--error:focus {
    box-shadow: 0 0 0 3px rgba(220, 38, 38, 0.1);
  }

  .auth-error-msg {
    font-size: 0.75rem;
    color: #dc2626;
    min-height: 1rem;
  }

  .auth-server-error {
    font-size: 0.8125rem;
    color: #dc2626;
    background-color: #fef2f2;
    border: 1px solid #fecaca;
    border-radius: 6px;
    padding: 0.5rem 0.75rem;
  }

  .auth-btn {
    margin-top: 0.25rem;
    padding: 0.65rem 1rem;
    font-size: 0.9375rem;
    font-family: inherit;
    font-weight: 500;
    color: #ffffff;
    background-color: #0f766e;
    border: none;
    border-radius: 6px;
    cursor: pointer;
    transition: background-color 0.15s ease;
  }

  .auth-btn:hover:not(:disabled) {
    background-color: #0d6460;
  }

  .auth-btn:disabled {
    opacity: 0.6;
    cursor: not-allowed;
  }

  .auth-links {
    display: flex;
    flex-direction: column;
    align-items: center;
    gap: 0.5rem;
    margin-top: 0.25rem;
  }

  .auth-link {
    font-size: 0.8125rem;
    color: #64748b;
    text-decoration: none;
    transition: color 0.15s ease;
  }

  .auth-link:hover {
    color: #0f172a;
    text-decoration: underline;
  }

  .auth-link--accent {
    color: #0f766e;
    font-weight: 500;
  }
</style>
