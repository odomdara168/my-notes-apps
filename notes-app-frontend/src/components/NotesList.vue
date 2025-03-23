<template>
  <div class="max-w-2xl mx-auto mt-10">
    <div class="flex justify-between items-center mb-4">
      <h2 class="text-2xl font-bold">Your Notes</h2>
      <button @click="auth.logout(); router.push('/login')" class="text-red-500 hover:text-red-700">Logout</button>
    </div>

    <div class="flex flex-col space-y-4 mb-8">
      <input v-model="title" placeholder="Note title" class="border p-2 rounded" />
      <textarea v-model="content" placeholder="Note content" class="border p-2 rounded"></textarea>
      <button @click="createNote" class="bg-green-500 text-white p-2 rounded hover:bg-green-600">
        Create Note
      </button>
    </div>

    <div v-for="note in notes" :key="note.id" class="bg-white p-4 mb-4 shadow rounded">
      <h3 class="font-bold text-xl">{{ note.title }}</h3>
      <p>{{ note.content }}</p>
      <div class="flex justify-between mt-2">
        <small>Created: {{ new Date(note.createdAt).toLocaleString() }}</small>
        <button @click="deleteNote(note.id)" class="text-red-500 hover:text-red-700">Delete</button>
      </div>
    </div>
  </div>
</template>
<script setup lang="ts">
import { ref, onMounted } from 'vue'
import api from '@/axios'
import { useAuthStore } from '@/store/auth'
import { useRouter } from 'vue-router'

const notes = ref([])
const title = ref('')
const content = ref('')
const auth = useAuthStore()
const router = useRouter()

const fetchNotes = async () => {
  try {
    const res = await api.get('/Notes')
    notes.value = res.data
  } catch (error) {
    console.error(error)
    router.push('/login')
  }
}

const createNote = async () => {
  try {
    await api.post('/Notes', { title: title.value, content: content.value })
    title.value = ''
    content.value = ''
    fetchNotes()
  } catch (error) {
    console.error(error)
  }
}

const deleteNote = async (id: number) => {
  await api.delete(`/Notes/${id}`)
  fetchNotes()
}

onMounted(() => {
  fetchNotes()
})
</script>


