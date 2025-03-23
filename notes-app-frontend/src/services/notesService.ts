import axios from 'axios'


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


export interface NoteDTO {
  id?: number
  title: string
  content: string
}


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

export const updateNote = async (id: number, note: NoteDTO) => {
  try {
    const response = await api.put(`/notes/${id}`, note)
    return response.data
  } catch (error: any) {
    console.error(`Error updating note ${id}:`, error.response?.data || error.message)
    throw error
  }
}


export const deleteNote = async (id: number) => {
  try {
    const response = await api.delete(`/notes/${id}`)
    return response.data
  } catch (error: any) {
    console.error(`Error deleting note ${id}:`, error.response?.data || error.message)
    throw error
  }
}


export default {
  getNotes,
  getNoteById,
  createNote,
  updateNote,
  deleteNote
}
