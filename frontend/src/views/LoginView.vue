<script lang="ts" setup>
  import { toTypedSchema } from "@vee-validate/zod";
  import { z } from "zod";
  import { useForm } from "vee-validate";
  import { ref } from "vue";
  import { useRouter, RouterLink } from "vue-router";
  import { useAuthStore } from "@/stores/auth";

  // import { ref } from "vue";

  const BACKEND_URL = import.meta.env.VITE_BACKEND_URL;
  const router = useRouter();
  const authStore = useAuthStore();
  const serverError = ref<string | null>(null);
  const isLoading = ref(false);

  const schema = toTypedSchema(
    z.object({
      email: z
        .string({ message: "El correo es obligatorio" })
        .email({ message: "El correo es inválido." }),
      password: z
        .string({ message: "La contraseña es obligatoria" })
        .min(8, { message: "La contraseña debe tener mínimo 8 caracteres" }),
    }),
  );
  const { errors, handleSubmit, defineField } = useForm({
    validationSchema: schema,
  });

  /*
  // emailAttrs | passwordAttrs → contiene los atributos y listeners que Vee Validate necesita para gestionar estos campos.
  */
  const [email, emailAttrs] = defineField("email");
  const [password, passwordAttrs] = defineField("password");

  const onSubmit = handleSubmit(async (values) => {
    try {
      serverError.value = null;
      isLoading.value = true;
      const request = await fetch(`${BACKEND_URL}/v1/auth/login`, {
        headers: {
          "Content-Type": "application/json;charset=utf-8",
        },
        body: JSON.stringify(values),
        method: "POST",
      });

      if (!request.ok) {
        const err = await request.json();
        serverError.value = err?.message ?? "Credenciales inválidas";
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
      <h1 class="auth-title">Login</h1>
      <p class="auth-subtitle">Ingresa a tu cuenta para continuar</p>

      <form class="auth-form" @submit.prevent="onSubmit">
        <fieldset class="auth-fieldset">
          <label class="auth-label" for="login-email">Email</label>
          <input
            id="login-email"
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
          <label class="auth-label" for="login-password">Contraseña</label>
          <input
            id="login-password"
            class="auth-input"
            type="password"
            placeholder="Mínimo 8 caracteres"
            v-model="password"
            v-bind="passwordAttrs"
            :class="{ 'auth-input--error': errors.password }"
          />
          <span class="auth-error-msg">{{ errors.password }}</span>
        </fieldset>

        <span v-if="serverError" class="auth-server-error">{{ serverError }}</span>

        <button class="auth-btn" type="submit" :disabled="isLoading">
          {{ isLoading ? "Ingresando..." : "Ingresar" }}
        </button>
      </form>

      <div class="auth-links">
        <a href="#" class="auth-link">¿Olvidaste tu contraseña?</a>
        <RouterLink to="/register" class="auth-link">
          ¿No tienes cuenta? <span class="auth-link--accent">Regístrate</span>
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
    /* Neutraliza el layout global del .main */
    margin-top: calc(-1 * var(--marginBody, 3.125rem));
  }

  .auth-card {
    background-color: #ffffff;
    border: 1px solid #e2e8f0;
    border-radius: 8px;
    box-shadow: 0 1px 4px rgba(0, 0, 0, 0.06);
    padding: 2.5rem 2rem;
    width: 100%;
    max-width: 400px;
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
