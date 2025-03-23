import { defineStore } from 'pinia'
import api from '@/axios'

export const useAuthStore = defineStore('auth', {
  state: () => ({
    token: localStorage.getItem('token') || '',
    username: ''
  }),
  actions: {
    async login(username: string, password: string) {
      const res = await api.post('/Auth/login', { username, password })
      this.token = res.data.token
      this.username = username
      localStorage.setItem('token', this.token)
    },
    async register(username: string, password: string) {
      const res = await api.post('/Auth/register', { username, password })
      return res
    },
    logout() {
      this.token = ''
      localStorage.removeItem('token')
    }
  }
})
