import  { useMicroService } from './../microservice/useMicroServiceRouter'

export interface IConfig {
    afterSlide: number,
    maxImages: number, 
    animation: string,
    svg: any,
    ppt: any
};

export const useConfigurator = () => {

    const services = useMicroService();

    const sendConfiguration = async (config: IConfig) => {
    
        const svgFormData = new FormData();
        svgFormData.append("file", config.svg);
        const svgResponse = await services.post('config', 'files', svgFormData);
        if (!svgResponse || svgResponse.status !== 200) return;

        const pptFormData = new FormData();
        pptFormData.append("file", config.ppt);
        const pptResponse = await services.post('config', 'files', pptFormData);
        if (!pptResponse || pptResponse.status !== 200) return;

        const configResponse = await services.put('config', 'configs', {
            "AfterSlide": config.afterSlide,
            "MaxImages": config.maxImages,
            "PptFileId": pptResponse.data.id,
            "SvgFileId": svgResponse.data.id,
            "Animation": config.animation
        });
        if (!configResponse || configResponse.status !== 200) return;

        return configResponse.data.id;

    };

    const sendStart = async (id: string) => {

        return await services.post('config', `start/${id}`);
        
    };

    return {
        sendConfiguration,
        sendStart
    }


};