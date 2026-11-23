import { ref } from 'vue'
import { defineStore } from 'pinia'
import { useRouter } from 'vue-router'

export interface AuthUser {
  name: string
  email: string
  token: string
}

export const useAuthStore = defineStore('auth', () => {
  const router = useRouter()
  const user = ref<AuthUser | null>(
    JSON.parse(localStorage.getItem('auth_user') ?? 'null')
  )

  const isAuthenticated = ref<boolean>(!!user.value)

  function setUser(data: AuthUser) {
    user.value = data
    isAuthenticated.value = true
    localStorage.setItem('auth_user', JSON.stringify(data))
  }

  function logout() {
    user.value = null
    isAuthenticated.value = false
    localStorage.removeItem('auth_user')
    router.push('/login')
  }

  return { user, isAuthenticated, setUser, logout }
})
