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


export interface UserDTO {
  username: string
  password: string 
}

// ✅ Register function
export const register = async (user: UserDTO) => {
  try {
    const response = await api.post('/auth/register', user)
    console.log('Registration success:', response.data)
    return response.data
  } catch (error: any) {
    console.error('Registration error:', error.response?.data || error.message)
    throw error
  }
}

// ✅ Login function
export const login = async (user: UserDTO) => {
    try {
      const response = await api.post('/auth/login', user)
  
      const token = response.data.token
      if (token) {
        localStorage.setItem('token', token)
        
        api.defaults.headers.common['Authorization'] = `Bearer ${token}`
      }
  
      return response.data // token, user info if you send it
    } catch (error: any) {
      console.error('Login error:', error.response?.data || error.message)
      throw error
    }
  }
  

// ✅ Logout
export const logout = () => {
  localStorage.removeItem('token')
  delete api.defaults.headers.common['Authorization']
}

// ✅ Check auth
export const isAuthenticated = (): boolean => {
  const token = localStorage.getItem('token')
  return !!token
}

export default {
  register,
  login,
  logout,
  isAuthenticated
}
