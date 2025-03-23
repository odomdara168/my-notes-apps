<template>
  <div class="min-h-full bg-gradient-to-r from-gray-200 to-gray-300">
    <!-- Navigation -->
    <nav class="bg-gradient-to-r from-orange-500 to-red-500 shadow-lg">
      <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
        <div class="flex items-center justify-between h-16">
          <div class="flex items-center">
            <div class="text-white text-3xl font-semibold">My Notes</div>
          </div>
          <button @click="logout" class="text-white px-4 py-2 rounded-md hover:bg-gray-700 transition">Logout</button>
        </div>
      </div>
    </nav>

    <!-- Main Content -->
    <div class="max-w-4xl mx-auto p-8 bg-white rounded-lg shadow-lg space-y-6 mt-8">
      <!-- Search and Sort -->
      <div v-if="!viewMode" class="flex flex-wrap gap-4 mb-6">
        <input v-model="searchTitle" @input="handleSearch" placeholder="Search by title..." class="flex-1 p-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-indigo-500" />
        <select v-model="sortBy" @change="handleSortChange" class="p-2 border border-gray-300 rounded-lg">
          <option value="CreatedAt">Sort by Created At</option>
          <option value="UpdatedAt">Sort by Updated At</option>
          <option value="Title">Sort by Title</option>
        </select>
        <select v-model="sortOrder" @change="handleSortChange" class="p-2 border border-gray-300 rounded-lg">
          <option value="desc">Descending</option>
          <option value="asc">Ascending</option>
        </select>
      </div>

      <!-- Create New Note -->
      <form @submit.prevent="handleCreate" class="bg-gray-100 p-6 rounded-xl shadow-md space-y-4">
        <h3 class="text-lg font-semibold text-gray-800">Create a New Note</h3>

        <div>
          <label class="block text-gray-600 font-medium mb-1">Title</label>
          <input v-model="newNoteTitle" placeholder="Enter note title..." class="w-full p-3 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-purple-500" />
        </div>

        <div>
          <label class="block text-gray-600 font-medium mb-1">Content</label>
          <textarea v-model="newNoteContent" placeholder="Write your note here..." class="w-full p-3 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-purple-500 resize-y min-h-[120px] max-h-[300px]"></textarea>
        </div>

        <p v-if="createErrorMessage" class="text-red-600 text-sm">{{ createErrorMessage }}</p>

        <button type="submit" class="w-full bg-green-600 text-white px-4 py-2 rounded-lg shadow-md hover:bg-green-700 transition-transform transform hover:scale-105">Add Note</button>
      </form>

      <!-- Notes List -->
      <div v-if="notes.length === 0" class="text-gray-500 text-center">No notes found.</div>
      <div v-for="note in notes" :key="note.id" class="bg-yellow-100 border border-yellow-200 p-6 rounded-lg mb-4 shadow-md hover:bg-yellow-200 transition-transform transform hover:shadow-xl">
        <h2 class="text-xl font-bold text-gray-800 mb-2">{{ note.title }}</h2>
        <p class="text-gray-600 text-sm mb-4">Created: {{ formatDate(note.createdAt) }}</p>

        <div v-if="viewingNoteId === note.id" class="mb-4">
          <p class="text-gray-700 whitespace-pre-wrap break-words mb-2">{{ note.content }}</p>
          <p class="text-gray-500 text-sm">Last updated: {{ formatDate(note.updatedAt) }}</p>
        </div>

        <div class="flex flex-wrap gap-2">
          <button @click="toggleNoteView(note.id)" class="bg-indigo-500 text-white px-3 py-1 rounded-lg hover:bg-indigo-600 transition">
            {{ viewingNoteId === note.id ? 'Hide' : 'View' }}
          </button>

          <!-- Edit/Delete buttons -->
          <template v-if="editingNoteId !== note.id">
            <button @click="startEdit(note)" class="bg-yellow-600 text-white px-3 py-1 rounded-lg hover:bg-yellow-700 transition">Edit</button>
            <button @click="handleDelete(note.id)" class="bg-red-600 text-white px-3 py-1 rounded-lg hover:bg-red-700 transition">Delete</button>
          </template>

          <!-- Edit form -->
          <div v-else class="w-full mt-4">
            <input v-model="editNoteTitle" class="w-full p-3 border border-gray-300 rounded-lg mb-2 focus:outline-none focus:ring-2 focus:ring-indigo-500" />
            <textarea v-model="editNoteContent" class="w-full p-3 border border-gray-300 rounded-lg mb-2 focus:outline-none focus:ring-2 focus:ring-indigo-500 resize-y min-h-[100px] max-h-[300px]"></textarea>
            <div v-if="editErrorMessage" class="text-red-600 mt-2">{{ editErrorMessage }}</div>

            <div class="flex flex-wrap gap-2">
              <button @click="handleUpdate(note.id)" class="bg-blue-600 text-white px-3 py-1 rounded-lg hover:bg-blue-700 transition">Save</button>
              <button @click="cancelEdit" class="px-3 py-1 rounded-lg border border-gray-300 hover:bg-gray-100 transition">Cancel</button>
            </div>
          </div>
        </div>
      </div>

      <!-- Pagination Controls -->
      <div v-if="pagination.totalPages > 1" class="flex justify-between mt-6">
        <button
          :disabled="pagination.currentPage === 1"
          @click="changePage(pagination.currentPage - 1)"
          class="bg-blue-600 text-white px-4 py-2 rounded-lg shadow-md disabled:opacity-50 hover:bg-blue-700 transition"
        >
          Previous
        </button>
        <div class="flex items-center space-x-4">
          <span>Page {{ pagination.currentPage }} of {{ pagination.totalPages }}</span>
        </div>
        <button
          :disabled="pagination.currentPage === pagination.totalPages"
          @click="changePage(pagination.currentPage + 1)"
          class="bg-blue-600 text-white px-4 py-2 rounded-lg shadow-md disabled:opacity-50 hover:bg-blue-700 transition"
        >
          Next
        </button>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue';
import notesService, { NoteDTO } from '@/services/notesService';
import { useRouter } from 'vue-router';

const notes = ref<NoteDTO[]>([]);
const pagination = ref({
  currentPage: 1,
  pageSize: 15,
  totalPages: 1,
  totalRecords: 0
});

const searchTitle = ref('');
const sortBy = ref('CreatedAt');
const sortOrder = ref('desc');

const newNoteTitle = ref('');
const newNoteContent = ref('');
const createErrorMessage = ref('');

const editNoteTitle = ref('');
const editNoteContent = ref('');
const editErrorMessage = ref('');

const editingNoteId = ref<number | null>(null);
const viewingNoteId = ref<number | null>(null);
const router = useRouter();

const fetchNotes = async () => {
  try {
    const response = await notesService.getNotes({
      title: searchTitle.value,
      sortBy: sortBy.value,
      sortOrder: sortOrder.value,
      page: pagination.value.currentPage
    });

    notes.value = response.data;
    pagination.value = response.pagination;
  } catch (error: any) {
    console.error('Error fetching notes:', error);
    notes.value = [];
    if (error.response?.status === 401) {
      router.push('/login');
    }
  }
};

onMounted(fetchNotes);

const handleCreate = async () => {
  createErrorMessage.value = '';
  if (!newNoteTitle.value) {
    createErrorMessage.value = 'Title is required.';
    return;
  }
  try {
    await notesService.createNote({
      title: newNoteTitle.value,
      content: newNoteContent.value
    });

    newNoteTitle.value = '';
    newNoteContent.value = '';
    pagination.value.currentPage = 1;
    fetchNotes();
  } catch (error) {
    console.error('Error creating note:', error);
  }
};

const startEdit = (note: NoteDTO) => {
  editingNoteId.value = note.id || null;
  editNoteTitle.value = note.title;
  editNoteContent.value = note.content;
};

const cancelEdit = () => {
  editingNoteId.value = null;
  editNoteTitle.value = '';
  editNoteContent.value = '';
};

const handleUpdate = async (id: number | undefined) => {
  if (!id) return;

  if (!editNoteTitle.value) {
    editErrorMessage.value = 'Title is required.';
    return;
  }

  try {
    await notesService.updateNote(id, {
      title: editNoteTitle.value,
      content: editNoteContent.value
    });

    cancelEdit();
    fetchNotes();
  } catch (error) {
    console.error('Error updating note:', error);
  }
};

const handleDelete = async (id: number | undefined) => {
  if (!id) return;

  try {
    await notesService.deleteNote(id);
    fetchNotes();
  } catch (error) {
    console.error('Error deleting note:', error);
  }
};

const handleSearch = () => {
  pagination.value.currentPage = 1;
  fetchNotes();
};

const handleSortChange = () => {
  pagination.value.currentPage = 1;
  fetchNotes();
};

const toggleNoteView = (id: number | undefined) => {
  if (!id) return;
  viewingNoteId.value = viewingNoteId.value === id ? null : id;
};

const logout = () => {
  router.push('/');
};

const formatDate = (date: string | Date) => {
  return new Date(date).toLocaleString();
};

const changePage = (page: number) => {
  pagination.value.currentPage = page;
  fetchNotes();
};
</script>

<style scoped>
/* Custom styles if needed */
</style>
