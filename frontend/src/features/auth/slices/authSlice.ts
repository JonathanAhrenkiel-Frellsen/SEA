import { createSlice } from '@reduxjs/toolkit';

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
    selectUser: (state): any => state.user,
    selectToken: (state) => state.token,
  }
});

export const { setCredentials, logoutAction } = authSlice.actions;
export const { selectUser, selectToken } = authSlice.selectors;
export default authSlice.reducer;
