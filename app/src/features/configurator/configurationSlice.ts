import { createSlice, PayloadAction } from '@reduxjs/toolkit'

export type FileInformation = {
    _id: string,
    fileId: string,
    userId: string,
    name: string,
};
export type StatusInformation = {
    _id: string,
    configId: string,
    status: string
};

export interface ConfigurationResponse {
    id: string,
    afterSlide : number, 
    maxImages : number, 
    pptFileId : string, 
    svgFileId : string, 
    animation : string, 
    pptFile: FileInformation[], 
    svgFile: FileInformation[],
    status: StatusInformation[]
};

export interface Configuration {
    ID: string,
    AfterSlide : number, 
    MaxImages : number, 
    PptFileId : string, 
    SvgFileId : string, 
    Animation : string, 
    PptFile?: FileInformation, 
    SvgFile?: FileInformation,
    Status?: string
};

type ConfigurationState = {
    configs:  { [id: string]: Configuration },
    downloaded: boolean
};

type StatusChange = {
    configID: string,
    newStatus:string
};

const initialConfigurationState: ConfigurationState = {
    configs: { },
    downloaded: false
};
const firstOrUndefined = (arr) => {
    if (!Array.isArray(arr) || arr.length <= 0) return undefined;
    return arr[0];
}

const firstValueOrUndefined = (arr) => {
    if (!Array.isArray(arr) || arr.length <= 0) return undefined;
    return arr[0].status;
};

const convertResponseToConfiguration = (configResponse: any): Configuration => {
    return {
        ID : configResponse.id, 
        AfterSlide : configResponse.afterSlide, 
        MaxImages : configResponse.maxImages, 
        PptFileId : configResponse.pptFileId, 
        SvgFileId : configResponse.svgFileId, 
        Animation : configResponse.animation, 
        PptFile: firstOrUndefined(configResponse.pptFile), 
        SvgFile: firstOrUndefined(configResponse.svgFile),
        Status: firstValueOrUndefined(configResponse.status)
    };
}

const configurationSlice = createSlice({
  name: 'config',
  initialState: initialConfigurationState,
  reducers: {

    setConfiguration: (state, action: PayloadAction<any>) => {

        const configResponse = action.payload;
        state.configs[configResponse.id] = convertResponseToConfiguration(configResponse);
    },


    setConfigurationStatus: (state, action: PayloadAction<StatusChange>) => {
        const statusChange = action.payload;
        state.configs[statusChange.configID].Status = statusChange.newStatus;
    },

    setDownloaded: (state, action: PayloadAction<boolean>) => {
        const statusChange = action.payload;
        state.downloaded = statusChange;
    }

  }
})

export const { setConfiguration, setConfigurationStatus,setDownloaded } = configurationSlice.actions

export default configurationSlice.reducer
