import axios from 'axios';

export const axiosClient = axios.create({
    baseURL: 'https://localhost:7019/api', 
    
    withCredentials: true, 
    
    headers: {
        'Content-Type': 'application/json'
    }
});