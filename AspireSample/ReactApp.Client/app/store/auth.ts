import { createSlice } from "@reduxjs/toolkit";
import { GUEST_USER } from "~/auth/contants";

export const authSlice = createSlice({
    name: 'auth',
    initialState: {
        user: GUEST_USER,
    },
    reducers: {
        login() {
        },
        logout() {
        },
    },
    selectors:{
        hasRole: (state, roles: string[]) => {
            return roles.some(role => state.user.roles.includes(role));
        },
        isAuthenticated(state){
            return state.user.id !== GUEST_USER.id;
        }
    }
})