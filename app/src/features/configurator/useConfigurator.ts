
import { useSelector } from 'react-redux'
import { useMicroService } from './../microservice/useMicroServiceRouter'
import { AppDispatch, RootState } from "../../reducers/store";
import { setConfiguration, setConfigurationStatus, setDownloaded } from "./configurationSlice";
import { config } from 'process';

export const useConfigurator = () => {

    const services = useMicroService();
    const { configs, downloaded } = useSelector( (state: RootState) => state.config );

    const getFileNames = () => async (dispatch: AppDispatch) => {

        //if (!downloaded) {

            console.log("Get file names");

            const response = await services.get('config', 'configs');
            if (response === undefined || response.status !== 200) return;

            console.log(response.data.configs);
            for (let config of response.data.configs) {
                console.log(config);
                dispatch(setConfiguration(config));
                console.log(configs);
            }

            dispatch(setDownloaded(true));

        //}

    };

    const updateStatus = () => async (dispatch: AppDispatch) => {

        for (let key in configs) {
            const config = configs[key];

            if (!config.Status || config.Status === "Done") continue;

            const response = await services.get('status', `${config.ID}`);
            console.log(response);
            if (response === undefined || response.status !== 200) return;

            dispatch(setConfigurationStatus({
                configID: config.ID,
                newStatus: response.data
            }));

        }

    };



    return {
        configs,
        getFileNames,
        updateStatus,
    };

};