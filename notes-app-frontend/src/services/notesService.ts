import axios from 'axios'

// ✅ Axios instance with baseURL and interceptor
const api = axios.create({
  baseURL: 'http://localhost:5122/api'
})

api.interceptors.request.use(config => {
  const token = localStorage.getItem('token')
  if (token && config.headers) {
    config.headers.Authorization = `Bearer ${token}`
  }
  return config
})

// ✅ Note DTO - adjust if your backend changes
export interface NoteDTO {
  id?: number
  title: string
  content: string
}

// ✅ Get notes (supports search, sorting, pagination)
export const getNotes = async (params?: {
  title?: string
  sortBy?: string
  sortOrder?: string
  page?: number
}) => {
  try {
    const response = await api.get('/notes', { params })
    return response.data
  } catch (error: any) {
    console.error('Error fetching notes:', error.response?.data || error.message)
    throw error
  }
}

// ✅ Get single note by ID (optional, but keeping it here)
export const getNoteById = async (id: number) => {
  try {
    const response = await api.get(`/notes/${id}`)
    return response.data
  } catch (error: any) {
    console.error(`Error fetching note ${id}:`, error.response?.data || error.message)
    throw error
  }
}

// ✅ Create new note
export const createNote = async (note: NoteDTO) => {
  try {
    const response = await api.post('/notes', note)
    return response.data
  } catch (error: any) {
    console.error('Error creating note:', error.response?.data || error.message)
    throw error
  }
}

// ✅ Update existing note
export const updateNote = async (id: number, note: NoteDTO) => {
  try {
    const response = await api.put(`/notes/${id}`, note)
    return response.data
  } catch (error: any) {
    console.error(`Error updating note ${id}:`, error.response?.data || error.message)
    throw error
  }
}

// ✅ Delete note
export const deleteNote = async (id: number) => {
  try {
    const response = await api.delete(`/notes/${id}`)
    return response.data
  } catch (error: any) {
    console.error(`Error deleting note ${id}:`, error.response?.data || error.message)
    throw error
  }
}

// ✅ Export for easier imports
export default {
  getNotes,
  getNoteById,
  createNote,
  updateNote,
  deleteNote
}
