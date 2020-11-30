import { TextField, BaseTextFieldProps } from '@material-ui/core';
import React, { FC } from 'react'

interface FileUploaderProps extends BaseTextFieldProps {

};

export const FileUploader: FC<FileUploaderProps> = ({ ...rest}: FileUploaderProps) => {


    return (
        <TextField {...rest} type="file" />
    );

};