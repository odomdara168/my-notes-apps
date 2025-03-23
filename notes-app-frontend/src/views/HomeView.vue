<template>
  <section class="gradient-form h-full bg-neutral-200 dark:bg-neutral-700">
    <div class="container h-full p-10">
      <div class="flex h-full flex-wrap items-center justify-center text-neutral-800 dark:text-neutral-200">
        <div class="w-full">
          <div class="block rounded-lg bg-white shadow-lg dark:bg-neutral-800">
            <div class="g-0 lg:flex lg:flex-wrap">
              <!-- Left column container -->
              <div class="px-4 md:px-0 lg:w-6/12">
                <div class="md:mx-6 md:p-12">
                  <!-- Logo -->
                  <div class="text-center">
                    <h1 class="text-3xl font-bold mb-4">Welcome to NoteWorthy!</h1>
                  </div>

                  <!-- Form -->
                  <form @submit.prevent="handleLogin">
                    <p class="mb-4">Please login to your account</p>

                    <!-- Username input -->
                    <div class="relative mb-4">
                      <input
                        type="text"
                        class="peer block min-h-[auto] w-full rounded border-0 bg-transparent px-3 py-[0.32rem] leading-[1.6] outline-none transition-all duration-200 ease-linear focus:ring-0"
                        id="exampleFormControlInput1"
                        placeholder=" "
                        v-model="username"
                        required
                      />
                      <label
                        v-if="!username"
                        for="exampleFormControlInput1"
                        class="pointer-events-none absolute left-3 top-2 mb-0 max-w-[90%] origin-[0_0] truncate pt-[0.37rem] leading-[1.6] text-neutral-500 transition-all duration-200 ease-out"
                      >
                        Username
                      </label>
                    </div>

                    <!-- Password input -->
                    <div class="relative mb-4">
                      <input
                        type="password"
                        class="peer block min-h-[auto] w-full rounded border-0 bg-transparent px-3 py-[0.32rem] leading-[1.6] outline-none transition-all duration-200 ease-linear focus:ring-0"
                        id="exampleFormControlInput11"
                        placeholder=" "
                        v-model="password"
                        required
                      />
                      <label
                        v-if="!password"
                        for="exampleFormControlInput11"
                        class="pointer-events-none absolute left-3 top-2 mb-0 max-w-[90%] origin-[0_0] truncate pt-[0.37rem] leading-[1.6] text-neutral-500 transition-all duration-200 ease-out"
                      >
                        Password
                      </label>
                    </div>

                    <!-- Error and Success Messages -->
                    <div v-if="errorMessage" class="text-red-500 mt-4">{{ errorMessage }}</div>
                    <div v-if="successMessage" class="text-green-500 mt-4">{{ successMessage }}</div>

                    <!-- Submit button -->
                    <div class="mb-12 pb-1 pt-1 text-center">
                      <button
                        class="mb-3 inline-block w-full rounded px-6 pb-2 pt-2.5 text-xs font-medium uppercase leading-normal text-white shadow-dark-3 transition duration-150 ease-in-out hover:shadow-dark-2 focus:shadow-dark-2 focus:outline-none"
                        type="submit"
                        style="background: linear-gradient(to right, #ee7724, #d8363a, #dd3675, #b44593);"
                      >
                        Log in
                      </button>

                      <!-- Forgot password link -->
                      <a href="#!" class="text-sm text-neutral-600 hover:text-neutral-900 dark:text-neutral-400 dark:hover:text-neutral-100">
                        Forgot password?
                      </a>
                    </div>

                    <!-- Register button -->
                    <div class="flex items-center justify-between pb-6">
                      <p class="mb-0 me-2 text-sm text-neutral-600 dark:text-neutral-400">
                        Don't have an account?
                      </p>
                      <router-link
                        to="/register"
                        class="inline-block rounded border-2 border-neutral-200 px-6 pb-[6px] pt-2 text-xs font-medium uppercase leading-normal text-neutral-200 transition duration-150 ease-in-out hover:border-neutral-300 hover:bg-neutral-100/50 hover:text-neutral-700"
                      >
                        Register
                      </router-link>
                    </div>
                  </form>
                </div>
              </div>

              <!-- Right column container -->
              <div class="flex items-center rounded-b-lg lg:w-6/12 lg:rounded-e-lg lg:rounded-bl-none"
                style="background: linear-gradient(to right, #ee7724, #d8363a, #dd3675, #b44593)">
                <div class="px-4 py-6 text-white md:mx-6 md:p-12">
                  <h4 class="mb-6 text-xl font-semibold">
                    Welcome to NoteWorthy
                  </h4>
                  <p class="text-sm">
                    A place to store and organize your notes securely
                  </p>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  </section>
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
      password: password.value
    })

    successMessage.value = 'Login successful! Redirecting...'
    
    // Redirect to the notes page
    setTimeout(() => {
      router.push('/notes')
    }, 1000)

    console.log('Login result:', result)
  } catch (error: any) {
    errorMessage.value = error.response?.data?.message || 'Login failed'
    console.error('Login error:', error)
  }
}
</script>

<style scoped>
.gradient-form {
  min-height: 100vh;
};

.dark-mode .gradient-form {
  background: #1a1a1a;
}
</style>