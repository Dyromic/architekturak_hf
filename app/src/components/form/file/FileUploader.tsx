import React, { createRef, useState } from 'react';
import { makeStyles, Theme, createStyles } from '@material-ui/core/styles';
import IconButton from '@material-ui/core/IconButton';
import PublishIcon from '@material-ui/icons/Publish';
import { FormControl, InputAdornment, InputLabel, OutlinedInput, OutlinedInputProps } from '@material-ui/core';
import clsx from 'clsx';

const useStyles = makeStyles((theme: Theme) =>
  createStyles({
    formControl: {
        cursor: "pointer"
    },
    margin: {
      margin: theme.spacing(1),
    },
    withoutLabel: {
      marginTop: theme.spacing(3),
    },
    textField: {
    },
    fileUpload: {
        display: "none"
    },
    pointer: {
        cursor: "pointer",
        "& input": {
            cursor: "pointer"
        }
    },
  }),
);

export interface FileUploaderProps extends OutlinedInputProps {
    id: string,
    formats?: string,
    multiple?: boolean,
    fullWidth?: boolean,
    onFileSelect?: (files) => any
};

export const FileUploader = ({id, placeholder, formats, multiple, fullWidth, onFileSelect, ...props}: FileUploaderProps) => {
    const classes = useStyles();
    // htmlFor="outlined-adornment-password"
    // id="outlined-adornment-password"
    const [filePath, setFilePath] = useState<string>("");

    const fileRef = createRef<HTMLInputElement>();

    const onInputClick = (event) => {
        fileRef.current?.click();
        event?.stopPropagation();
    };

    const onFileSelectHandler = (event) => {
        if (event.target.files) {
            setFilePath(event.target.files[0].name);
        }
        if (onFileSelect) onFileSelect(event.target.files);
    };

    return (     
            <FormControl className={clsx(classes.pointer, classes.margin, classes.textField)} fullWidth={fullWidth} variant="outlined">
                <InputLabel className={classes.pointer} >{placeholder}</InputLabel>
                <label htmlFor={id}>
                    <OutlinedInput className={classes.pointer} readOnly fullWidth={fullWidth} {...props} onClick={onInputClick} disabled 
                        value={filePath}
                        type={'text'}
                        endAdornment={
                            <InputAdornment position="end">

                                <IconButton onClick={onInputClick}><PublishIcon /></IconButton>

                            </InputAdornment>
                        }
                        label={placeholder}
                    />
                </label>
                <input ref={fileRef} className={classes.fileUpload} accept={formats} id={id} multiple={multiple} type="file" onChange={onFileSelectHandler}/>
            </FormControl>
    );
}