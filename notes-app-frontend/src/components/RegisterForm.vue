<template>
  <section class="gradient-form h-full bg-neutral-200 dark:bg-neutral-700">
    <div class="container h-full p-10">
      <div class="flex h-full flex-wrap items-center justify-center text-neutral-800 dark:text-neutral-200">
        <div class="w-full">
          <div class="block rounded-lg bg-white shadow-lg dark:bg-neutral-800">
            <div class="g-0 lg:flex lg:flex-wrap">
              <!-- Left column container-->
              <div class="px-4 md:px-0 lg:w-6/12">
                <div class="md:mx-6 md:p-12">
                  <form @submit.prevent="handleRegister">
                    <p class="mb-4">Please register an account</p>

                    <!-- Username input -->
                    <div class="relative mb-4">
                      <input
                        type="text"
                        v-model.trim="username"
                        :placeholder="username ? '' : 'Username'"
                        class="peer block min-h-[auto] w-full rounded border-0 bg-transparent px-3 py-[0.32rem] leading-[1.6] outline-none transition-all duration-200 ease-linear focus:placeholder:opacity-100 peer-focus:text-primary dark:text-white dark:placeholder:text-neutral-300 dark:peer-focus:text-primary"
                        id="username"
                        required
                      />
                    </div>

                    <!-- Password input -->
                    <div class="relative mb-4">
                      <input
                        type="password"
                        v-model.trim="password"
                        :placeholder="password ? '' : 'Password'"
                        class="peer block min-h-[auto] w-full rounded border-0 bg-transparent px-3 py-[0.32rem] leading-[1.6] outline-none transition-all duration-200 ease-linear focus:placeholder:opacity-100 peer-focus:text-primary dark:text-white dark:placeholder:text-neutral-300 dark:peer-focus:text-primary"
                        id="password"
                        required
                      />
                    </div>

                    <!-- Submit button -->
                    <div class="mb-12 pb-1 pt-1 text-center">
                      <button
                        type="submit"
                        class="mb-3 inline-block w-full rounded px-6 pb-2 pt-2.5 text-xs font-medium uppercase leading-normal text-white shadow-dark-3 transition duration-150 ease-in-out hover:shadow-dark-2 focus:shadow-dark-2 focus:outline-none focus:ring-0 active:shadow-dark-2 dark:shadow-black/30 dark:hover:shadow-dark-strong dark:focus:shadow-dark-strong dark:active:shadow-dark-strong"
                        style="background: linear-gradient(to right, #ee7724, #d8363a, #dd3675, #b44593);"
                      >
                        Sign up
                      </button>
                    </div>

                    <!-- Error message -->
                    <div v-if="errorMessage" class="text-red-500 mt-4 text-center">
                      {{ errorMessage }}
                    </div>

                    <!-- Success message -->
                    <div v-if="successMessage" class="text-green-500 mt-4 text-center">
                      {{ successMessage }}
                    </div>
                  </form>
                </div>
              </div>

              <!-- Right column container with background and description -->
              <div
                class="flex items-center rounded-b-lg lg:w-6/12 lg:rounded-e-lg lg:rounded-bl-none"
                style="background: linear-gradient(to right, #ee7724, #d8363a, #dd3675, #b44593)"
              >
                <div class="px-4 py-6 text-white md:mx-6 md:p-12">
                  <h4 class="mb-6 text-xl font-semibold">Sign Up For NoteWorthy</h4>
                  <p class="text-sm">
                    You’ve just made a great choice by signing up for NoteWorthy! We hope you’ll find it, well, *worthy* of your notes—no pun intended! 😉
                  </p>
                  <footer class="text-center text-gray-400 text-sm mt-10">
  <p>Created with ❤️ by Odom DARA</p>
</footer>

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

// Form state
const username = ref('')
const password = ref('')
const errorMessage = ref('')
const successMessage = ref('')

const router = useRouter()

const handleRegister = async () => {
  errorMessage.value = ''
  successMessage.value = ''

  // Quick validation (optional but good practice)
  if (!username.value || !password.value) {
    errorMessage.value = 'Username and password are required.'
    return
  }

  try {
    console.log('Registering user:', { username: username.value, password: password.value })

    // Call register API
    await authService.register({
      username: username.value,
      password: password.value
    })

    successMessage.value = 'Registration successful! Redirecting to Home...'
    errorMessage.value = ''

    // Redirect to HomeView after 1 second
    setTimeout(() => {
      router.push('/home') // Redirect to the HomeView
    }, 1000)
  } catch (error: any) {
    const backendError = error.response?.data?.message || error.response?.data || error.message
    errorMessage.value = `Registration failed: ${backendError}`
    console.error('Registration error:', backendError)
  }
}
</script>
