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
    status: string,
    resultFileId: string
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
    Status?: StatusInformation
};

type ConfigurationState = {
    configs:  { [id: string]: Configuration }
};

type StatusChange = {
    configID: string,
    newStatus: StatusInformation
};

const initialConfigurationState: ConfigurationState = {
    configs: { }
};
const firstOrUndefined = (arr) => {
    if (!Array.isArray(arr) || arr.length <= 0) return undefined;
    return arr[0];
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
        Status: firstOrUndefined(configResponse.status)
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
    }

  }
})

export const { setConfiguration, setConfigurationStatus } = configurationSlice.actions

export default configurationSlice.reducer
