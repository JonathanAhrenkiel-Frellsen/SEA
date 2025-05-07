import { createSlice } from '@reduxjs/toolkit';
import {ReduxUser} from "../types";

const authSlice = createSlice({
  name: 'auth',
  initialState: { user: null, token: null },
  reducers: {
    setCredentials: (state, action) => {
      state.user = action.payload.user;
      state.token = action.payload.token;
    },
    logoutAction: (state) => {
      state.user = null;
      state.token = null;
    },
  },
  selectors: {
    selectUser: (state): ReduxUser | null => state.user,
    selectToken: (state): string | null => state.token,
  }
});

export const { setCredentials, logoutAction } = authSlice.actions;
export const { selectUser, selectToken } = authSlice.selectors;
export default authSlice.reducer;
