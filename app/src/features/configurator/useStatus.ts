
import { useSelector } from 'react-redux'
import { useMicroService } from '../microservice/useMicroServiceRouter'
import { AppDispatch, RootState } from "../../reducers/store";
import { setConfiguration, setConfigurationStatus } from "./configurationSlice";

type ConfiguratorOptions = {
    updateMs: number,
}

export const useStatus = (options?: ConfiguratorOptions) => {

    const services = useMicroService();
    const { configs } = useSelector( (state: RootState) => state.config );

    const getConfigurations = () => async (dispatch: AppDispatch) => {

        const response = await services.get('config', 'configs');
        if (response === undefined || response.status !== 200) return;

        for (let config of response.data.configs) {
            dispatch(setConfiguration(config));
        }

    };

    const getConfiguration = async (id: string, dispatch: AppDispatch) => {

        const response = await services.get('config', `configs/${id}`);
        if (response === undefined || response.status !== 200) return;

        console.log(response.data);
        
        dispatch(setConfiguration(response.data.config));

    };

    const updateStatus = () => async (dispatch: AppDispatch) => {

        for (let key in configs) {
            const config = configs[key];

            if (!config.Status || config.Status === "Done") continue;

            const response = await services.get('status', `${config.ID}`);
            if (response === undefined || response.status !== 200) return;

            console.log(response.data);

            dispatch(setConfigurationStatus({
                configID: config.ID,
                newStatus: response.data
            }));

        }

    };


    return {
        configs,
        getConfiguration,
        getConfigurations,
        updateStatus,
    };

};