<template>
  <div class="min-h-full bg-gradient-to-r from-gray-200 to-gray-300">
    <!-- Navigation -->
    <nav class="bg-gradient-to-r from-[rgb(238,119,36)] to-[rgb(216,54,58)] shadow-lg">
      <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
        <div class="flex items-center justify-between h-16">
          <div class="flex items-center">
            <div class="flex-shrink-0 text-white text-3xl font-semibold">My Notes</div>
          </div>
          <div class="flex space-x-4">
            <button @click="logout" class="text-white px-4 py-2 rounded-md hover:bg-gray-700 transition">Logout</button>
          </div>
        </div>
      </div>
    </nav>

    <!-- Main Content -->
    <div class="max-w-4xl mx-auto p-8 bg-white rounded-lg shadow-lg space-y-6">
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

      <!-- Add Note Form -->
      <form @submit.prevent="handleCreate" class="mb-6">
        <div class="mb-4">
          <input v-model="newNoteTitle" placeholder="Title" class="w-full p-3 border border-gray-300 rounded-lg mb-2 focus:outline-none focus:ring-2 focus:ring-purple-500" />
          <textarea v-model="newNoteContent" placeholder="Content" class="w-full p-3 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-purple-500 resize-y min-h-[100px] max-h-[300px]"></textarea>
        </div>
        <button type="submit" class="bg-green-600 text-white px-4 py-2 rounded-lg shadow-md hover:bg-green-700 transition-transform transform hover:scale-105">Add Note</button>
        <p v-if="createErrorMessage" class="text-red-600 mt-2">{{ createErrorMessage }}</p> <!-- Show create error message -->
      </form>

      <!-- Notes List -->
      <div v-if="notes.length === 0" class="text-gray-500 text-center">No notes found.</div>
      <div v-for="note in notes" :key="note.id" class="bg-white border border-gray-200 p-4 rounded-lg mb-4 shadow-lg transition-transform transform hover:shadow-xl hover:scale-105">
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

          <template v-if="editingNoteId !== note.id">
            <button @click="startEdit(note)" class="bg-yellow-500 text-white px-3 py-1 rounded-lg hover:bg-yellow-600 transition">Edit</button>
            <button @click="handleDelete(note.id)" class="bg-red-600 text-white px-3 py-1 rounded-lg hover:bg-red-700 transition">Delete</button>
          </template>

          <div v-else class="w-full mt-4">
            <input v-model="editNoteTitle" class="w-full p-3 border border-gray-300 rounded-lg mb-2 focus:outline-none focus:ring-2 focus:ring-indigo-500" />
            <textarea v-model="editNoteContent" class="w-full p-3 border border-gray-300 rounded-lg mb-2 focus:outline-none focus:ring-2 focus:ring-indigo-500 resize-y min-h-[100px] max-h-[300px]"></textarea>
            <div v-if="editErrorMessage" class="text-red-600 mt-2">{{ editErrorMessage }}</div> <!-- Show edit error message -->
            <div class="flex flex-wrap gap-2">
              <button @click="handleUpdate(note.id)" class="bg-blue-600 text-white px-3 py-1 rounded-lg hover:bg-blue-700 transition">Save</button>
              <button @click="cancelEdit" class="px-3 py-1 rounded-lg border border-gray-300 hover:bg-gray-100 transition">Cancel</button>
            </div>
          </div>
        </div>
      </div>

      <!-- Pagination -->
      <div v-if="pagination.totalPages > 1" class="flex items-center justify-between border-t border-gray-200 bg-white px-4 py-3 sm:px-6 mt-8">
        <div class="hidden sm:flex sm:flex-1 sm:items-center sm:justify-between">
          <div>
            <p class="text-sm text-gray-700">
              Showing
              <span class="font-medium">{{ (pagination.currentPage - 1) * pagination.pageSize + 1 }}</span>
              to
              <span class="font-medium">{{ Math.min(pagination.currentPage * pagination.pageSize, pagination.totalRecords) }}</span>
              of
              <span class="font-medium">{{ pagination.totalRecords }}</span>
              results
            </p>
          </div>
          <div>
            <nav class="inline-flex -space-x-px rounded-md shadow-sm" aria-label="Pagination">
              <button @click="goToPage(pagination.currentPage - 1)" :disabled="pagination.currentPage === 1" class="relative inline-flex items-center rounded-l-md px-2 py-2 text-gray-400 ring-1 ring-gray-300 ring-inset hover:bg-gray-50 focus:outline-none">
                <span class="sr-only">Previous</span>
                <svg class="w-5 h-5" viewBox="0 0 20 20" fill="currentColor">
                  <path fill-rule="evenodd" d="M11.78 5.22a.75.75 0 0 1 0 1.06L8.06 10l3.72 3.72a.75.75 0 1 1-1.06 1.06l-4.25-4.25a.75.75 0 0 1 0-1.06l4.25-4.25a.75.75 0 0 1 1.06 0Z" clip-rule="evenodd" />
                </svg>
              </button>

              <span v-for="page in pagination.totalPages" :key="page">
                <button @click="goToPage(page)" :class="['relative inline-flex items-center px-4 py-2 text-sm font-semibold ring-1 ring-gray-300 ring-inset hover:bg-gray-50', pagination.currentPage === page ? 'bg-indigo-600 text-white' : 'text-gray-900']">{{ page }}</button>
              </span>

              <button @click="goToPage(pagination.currentPage + 1)" :disabled="pagination.currentPage === pagination.totalPages" class="relative inline-flex items-center rounded-r-md px-2 py-2 text-gray-400 ring-1 ring-gray-300 ring-inset hover:bg-gray-50 focus:outline-none">
                <span class="sr-only">Next</span>
                <svg class="w-5 h-5" viewBox="0 0 20 20" fill="currentColor">
                  <path fill-rule="evenodd" d="M8.22 5.22a.75.75 0 0 1 1.06 0l4.25 4.25a.75.75 0 0 1 0 1.06l-4.25 4.25a.75.75 0 0 1-1.06-1.06L11.94 10 8.22 6.28a.75.75 0 0 1 0-1.06Z" clip-rule="evenodd" />
                </svg>
              </button>
            </nav>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import notesService, { NoteDTO } from '@/services/notesService'
import { useRouter } from 'vue-router'

const notes = ref<NoteDTO[]>([]);  // Initialize as an empty array
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

// Error message for create note
const createErrorMessage = ref('');

// Error message for edit note
const editErrorMessage = ref('');

const editingNoteId = ref<number | null>(null);
const editNoteTitle = ref('');
const editNoteContent = ref('');

const viewingNoteId = ref<number | null>(null);
const viewMode = ref(false);

const router = useRouter();

const fetchNotes = async () => {
  try {
    const response = await notesService.getNotes({
      title: searchTitle.value,
      sortBy: sortBy.value,
      sortOrder: sortOrder.value,
      page: pagination.value.currentPage
    });

    notes.value = response.data;  // Ensure this is set correctly
    pagination.value = response.pagination;  // Ensure pagination is set correctly
  } catch (error: any) {
    console.error('Error fetching notes:', error);
    notes.value = [];  // Set notes to an empty array on error
    if (error.response?.status === 401) {
      router.push('/login');
    }
  }
};

onMounted(fetchNotes);

const handleCreate = async () => {
  // Reset error message
  createErrorMessage.value = '';

  // Check for empty title
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

  // Check if the title is empty
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

const goToPage = (page: number) => {
  if (page < 1 || page > pagination.value.totalPages) return;
  pagination.value.currentPage = page;
  fetchNotes();
};

const logout = () => {
  router.push('/');
};

const toggleNoteView = (id: number | undefined) => {
  if (!id) return;
  viewingNoteId.value = viewingNoteId.value === id ? null : id;
};

const formatDate = (date: string | Date) => {
  return new Date(date).toLocaleString();
};
</script>

<style scoped>
/* Optional: customize scrollbars */
</style>
