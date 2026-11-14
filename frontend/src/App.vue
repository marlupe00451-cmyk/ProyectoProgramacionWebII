
<script setup lang="ts">
import { ref } from 'vue'

interface Usuario {
  id: number
  nombre: string
  correo: string
}

const API_URL = 'http://localhost:5181'

const modo = ref<'login' | 'registro'>('login')
const nombre = ref('')
const correo = ref('')
const password = ref('')
const mensaje = ref('')
const cargando = ref(false)
const autenticado = ref(false)
const usuarios = ref<Usuario[]>([])

async function cargarUsuarios() {
  const respuesta = await fetch(`${API_URL}/api/Usuarios`, {
    method: 'GET',
    credentials: 'include'
  })

  if (!respuesta.ok) {
    throw new Error('No se pudo obtener la lista de usuarios')
  }

  usuarios.value = await respuesta.json()
}

async function enviarFormulario() {
  mensaje.value = ''
  cargando.value = true

  try {
    const endpoint = modo.value === 'login'
      ? '/api/Auth/login'
      : '/api/Usuarios'

    const datos = modo.value === 'login'
      ? { correo: correo.value, password: password.value }
      : {
          nombre: nombre.value,
          correo: correo.value,
          password: password.value
        }

    const respuesta = await fetch(`${API_URL}${endpoint}`, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json'
      },
      credentials: 'include',
      body: JSON.stringify(datos)
    })

    const resultado = await respuesta.json()

    if (!respuesta.ok) {
      throw new Error(resultado.mensaje || 'No se pudo completar la operación')
    }

    if (modo.value === 'registro') {
      mensaje.value = 'Usuario registrado correctamente. Ahora inicia sesión.'
      modo.value = 'login'
      nombre.value = ''
      password.value = ''
    } else {
      autenticado.value = true
      mensaje.value = 'Inicio de sesión correcto'
      password.value = ''
      await cargarUsuarios()
    }
  } catch (error) {
    mensaje.value = error instanceof Error
      ? error.message
      : 'Error al conectar con el servidor'
  } finally {
    cargando.value = false
  }
}

async function cerrarSesion() {
  try {
    const respuesta = await fetch(`${API_URL}/api/Auth/logout`, {
      method: 'POST',
      credentials: 'include'
    })

    if (!respuesta.ok) {
      throw new Error('No se pudo cerrar la sesión')
    }

    autenticado.value = false
    usuarios.value = []
    correo.value = ''
    password.value = ''
    mensaje.value = 'Sesión cerrada correctamente'
  } catch (error) {
    mensaje.value = error instanceof Error
      ? error.message
      : 'Error al cerrar sesión'
  }
}
</script>

<template>
  <main class="pagina">
    <section v-if="!autenticado" class="panel">
      <div class="encabezado">
        <div class="logo">PW</div>
        <h1>Mi Sistema</h1>
        <p>Bienvenido a nuestra plataforma</p>
      </div>

      <div class="pestanas">
        <button
          type="button"
          :class="{ activa: modo === 'login' }"
          @click="modo = 'login'; mensaje = ''"
        >
          Iniciar sesión
        </button>

        <button
          type="button"
          :class="{ activa: modo === 'registro' }"
          @click="modo = 'registro'; mensaje = ''"
        >
          Registrarse
        </button>
      </div>

      <form @submit.prevent="enviarFormulario">
        <div v-if="modo === 'registro'" class="campo">
          <label for="nombre">Nombre completo</label>
          <input
            id="nombre"
            v-model="nombre"
            type="text"
            placeholder="Escribe tu nombre"
            required
          />
        </div>

        <div class="campo">
          <label for="correo">Correo electrónico</label>
          <input
            id="correo"
            v-model="correo"
            type="email"
            placeholder="ejemplo@correo.com"
            required
          />
        </div>

        <div class="campo">
          <label for="password">Contraseña</label>
          <input
            id="password"
            v-model="password"
            type="password"
            placeholder="Escribe tu contraseña"
            required
          />
        </div>

        <button class="boton-principal" type="submit" :disabled="cargando">
          {{
            cargando
              ? 'Procesando...'
              : modo === 'login'
                ? 'Entrar al sistema'
                : 'Crear cuenta'
          }}
        </button>

        <p v-if="mensaje" class="mensaje" role="status">
          {{ mensaje }}
        </p>
      </form>

      <p class="pie">Proyecto de Programación Web II</p>
    </section>

    <section v-else class="panel panel-usuarios">
      <div class="barra-superior">
        <div>
          <h1>Panel de usuarios</h1>
          <p>Usuarios registrados en el sistema</p>
        </div>

        <button class="boton-salir" @click="cerrarSesion">
          Cerrar sesión
        </button>
      </div>

      <div class="resumen">
        <span>Total de usuarios registrados</span>
        <strong>{{ usuarios.length }}</strong>
      </div>

      <h2>Lista de usuarios</h2>

      <div class="tabla-contenedor">
        <table>
          <thead>
            <tr>
              <th>ID</th>
              <th>Nombre</th>
              <th>Correo electrónico</th>
            </tr>
          </thead>

          <tbody>
            <tr v-for="usuario in usuarios" :key="usuario.id">
              <td>{{ usuario.id }}</td>
              <td>{{ usuario.nombre }}</td>
              <td>{{ usuario.correo }}</td>
            </tr>

            <tr v-if="usuarios.length === 0">
              <td colspan="3">No hay usuarios registrados.</td>
            </tr>
          </tbody>
        </table>
      </div>

      <p v-if="mensaje" class="mensaje" role="status">
        {{ mensaje }}
      </p>

      <p class="pie">Proyecto de Programación Web II</p>
    </section>
  </main>
</template>

<style scoped>
* {
  box-sizing: border-box;
}

.pagina {
  min-height: 100vh;
  display: flex;
  align-items: center;
  justify-content: center;
  padding: 25px;
  background: linear-gradient(135deg, #e8efff, #d9e4fa);
  font-family: Arial, sans-serif;
}

.panel {
  width: 100%;
  max-width: 430px;
  padding: 35px;
  border-radius: 18px;
  background: white;
  box-shadow: 0 12px 35px rgba(30, 50, 90, 0.12);
}

.encabezado {
  text-align: center;
  margin-bottom: 28px;
}

.logo {
  width: 58px;
  height: 58px;
  display: grid;
  place-items: center;
  margin: 0 auto 15px;
  border-radius: 15px;
  background: #3157c8;
  color: white;
  font-size: 22px;
  font-weight: bold;
}

h1 {
  margin: 0 0 8px;
  color: #24345b;
}

.encabezado p,
.barra-superior p {
  color: #71809b;
}

.pestanas {
  display: flex;
  gap: 8px;
  margin-bottom: 25px;
}

.pestanas button {
  flex: 1;
  padding: 12px 5px;
  border: none;
  border-radius: 8px;
  background: #edf1fa;
  color: #52617d;
  cursor: pointer;
}

.pestanas button.activa {
  background: #3157c8;
  color: white;
}

.campo {
  margin-bottom: 18px;
}

.campo label {
  display: block;
  margin-bottom: 8px;
  color: #34415d;
  font-size: 14px;
  font-weight: bold;
}

.campo input {
  width: 100%;
  padding: 13px;
  border: 1px solid #d5ddeb;
  border-radius: 8px;
  outline: none;
  font-size: 14px;
}

.campo input:focus {
  border-color: #3157c8;
}

.boton-principal,
.boton-salir {
  padding: 13px;
  border: none;
  border-radius: 8px;
  background: #3157c8;
  color: white;
  font-weight: bold;
  cursor: pointer;
}

.boton-principal {
  width: 100%;
}

.boton-principal:disabled {
  opacity: 0.6;
  cursor: wait;
}

.mensaje {
  margin-top: 18px;
  padding: 12px;
  border-radius: 8px;
  background: #edf1fa;
  color: #24345b;
  text-align: center;
  font-size: 14px;
}

.panel-usuarios {
  max-width: 850px;
}

.barra-superior {
  display: flex;
  justify-content: space-between;
  align-items: center;
  gap: 15px;
  margin-bottom: 25px;
}

.barra-superior h1 {
  margin-bottom: 5px;
}

.barra-superior p {
  margin: 0;
}

.resumen {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 20px;
  margin-bottom: 25px;
  border-radius: 10px;
  background: #edf1fa;
  color: #34415d;
}

.resumen strong {
  font-size: 28px;
  color: #3157c8;
}

h2 {
  color: #24345b;
  font-size: 19px;
}

.tabla-contenedor {
  overflow-x: auto;
}

table {
  width: 100%;
  border-collapse: collapse;
  text-align: left;
}

th,
td {
  padding: 13px;
  border-bottom: 1px solid #e2e7f0;
  font-size: 14px;
}

th {
  background: #edf1fa;
  color: #34415d;
}

td {
  color: #52617d;
}

.pie {
  margin: 25px 0 0;
  color: #8792a8;
  font-size: 12px;
  text-align: center;
}
</style>
