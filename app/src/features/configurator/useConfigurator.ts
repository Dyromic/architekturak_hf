
import { useSelector } from 'react-redux'
import { useMicroService } from './../microservice/useMicroServiceRouter'
import { AppDispatch, RootState } from "../../reducers/store";
import { setFiles, SvgFile } from "./fileSlice";

export const useConfigurator = () => {

    const services = useMicroService();
    const { files } = useSelector( (state: RootState) => state.config );

    const getFileNames = () => async (dispatch: AppDispatch) => {

        const response = await services.get('config', 'files');
        if (response === undefined || response.status !== 200) return;

        const files = response.data as SvgFile[];
        dispatch(setFiles(files));

    };


    return {
        files,
        getFileNames,
    };

};