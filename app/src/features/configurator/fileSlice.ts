import { createSlice, PayloadAction } from '@reduxjs/toolkit'

export type SvgFile = {
    id: string,
    name: string,
    status: string
};

type FileState = {
    files: SvgFile[],
};

const initialFileState: FileState = {
    files: [],
};

const fileSlice = createSlice({
  name: 'file',
  initialState: initialFileState,
  reducers: {

    setFiles: (state, action: PayloadAction<SvgFile[]>) => {
        const files = action.payload;
        state.files = files;
    },
    addFile: (state, action: PayloadAction<SvgFile>) => {
        const file = action.payload;
        state.files = [...state.files, file];
    }

  }
})

export const { setFiles, addFile } = fileSlice.actions

export default fileSlice.reducer
