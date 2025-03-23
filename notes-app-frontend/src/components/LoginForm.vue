<template>
  <div class="flex justify-center items-center h-screen">
    <div class="bg-white p-8 rounded shadow-md w-full max-w-md">
      <h2 class="text-2xl font-bold mb-4">Login</h2>

      <form @submit.prevent="handleLogin">
        <div class="mb-4">
          <label for="username" class="block text-sm font-medium text-gray-700">Username</label>
          <input
            type="text"
            id="username"
            v-model="username"
            class="mt-1 block w-full p-2 border border-gray-300 rounded-md shadow-sm"
            required
          />
        </div>

        <div class="mb-4">
          <label for="password" class="block text-sm font-medium text-gray-700">Password</label>
          <input
            type="password"
            id="password"
            v-model="password"
            class="mt-1 block w-full p-2 border border-gray-300 rounded-md shadow-sm"
            required
          />
        </div>

        <button
          type="submit"
          class="w-full bg-blue-500 hover:bg-blue-600 text-white font-bold py-2 px-4 rounded focus:outline-none focus:shadow-outline"
        >
          Login
        </button>

        <div v-if="errorMessage" class="text-red-500 mt-4">{{ errorMessage }}</div>
        <div v-if="successMessage" class="text-green-500 mt-4">{{ successMessage }}</div>
      </form>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue'
import authService from '@/services/authService'
import { useRouter } from 'vue-router'

const username = ref('')
const password = ref('')
const errorMessage = ref('')
const successMessage = ref('')

const router = useRouter()

const handleLogin = async () => {
  errorMessage.value = ''
  successMessage.value = ''

  try {
    const result = await authService.login({
      username: username.value,
      password: password.value // ✅ Match your DTO
    })

    successMessage.value = 'Login successful! Redirecting...'

    // Redirect to the dashboard or home
    setTimeout(() => {
      router.push('/notes') // Change route as needed
    }, 1000)

    console.log('Login result:', result)
  } catch (error: any) {
    errorMessage.value = error.response?.data?.message || 'Login failed'
    console.error('Login error:', error)
  }
}
</script>
