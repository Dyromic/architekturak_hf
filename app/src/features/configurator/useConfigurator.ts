
import { useSelector } from 'react-redux'
import { useMicroService } from './../microservice/useMicroServiceRouter'
import { AppDispatch, RootState, useAppDispatch } from "../../reducers/store";
import { setConfiguration, setConfigurationStatus } from "./configurationSlice";
import { useEffect } from 'react';

type ConfiguratorOptions = {
    updateMs: number,
}

export const useConfigurator = (options?: ConfiguratorOptions) => {

    const dispatch = useAppDispatch();
    const services = useMicroService();
    const { configs } = useSelector( (state: RootState) => state.config );

    const getFileNames = () => async (dispatch: AppDispatch) => {

            const response = await services.get('config', 'configs');
            if (response === undefined || response.status !== 200) return;

            for (let config of response.data.configs) {
                dispatch(setConfiguration(config));
            }

    };

    const updateStatus = () => async (dispatch: AppDispatch) => {

        for (let key in configs) {
            const config = configs[key];

            if (!config.Status || config.Status === "Done") continue;

            const response = await services.get('status', `${config.ID}`);
            if (response === undefined || response.status !== 200) return;

            dispatch(setConfigurationStatus({
                configID: config.ID,
                newStatus: response.data
            }));

        }

    };
    
    useEffect(() => {

        dispatch(getFileNames());

        const updateTable = () => {
            dispatch(updateStatus());
        }
        
        const handle = setInterval(updateTable, options?.updateMs ?? 3000);

        return () => {
            clearInterval(handle);
        };

    // eslint-disable-next-line react-hooks/exhaustive-deps
    }, [dispatch, services.serviceAvailable]);



    return {
        configs,
        getFileNames,
        updateStatus,
    };

};