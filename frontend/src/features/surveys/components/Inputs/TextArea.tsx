import React from 'react';
import { useDispatch, useSelector } from 'react-redux';
import { setTextValue } from '../../slices/surveySlice';
import {RootState} from "../../../../app/store";

interface TextAreaProps {
    name: string;
    label?: string;
    placeholder?: string;
}

const TextArea: React.FC<TextAreaProps> = ({ name, label, placeholder }) => {
    const dispatch = useDispatch();
    const value = useSelector((state: RootState) => state.surveyForm[name] || '');

    const handleChange = (e: React.ChangeEvent<HTMLTextAreaElement>) => {
        dispatch(setTextValue({ name, value: e.target.value }));
    };

    return (
        <div className="flex flex-col gap-1">
            {label && <label className="text-white">{label}</label>}
            <textarea
                value={value}
                onChange={handleChange}
                placeholder={placeholder}
                className="bg-transparent text-white border border-white p-2 outline-none resize-none box-border"
            />
        </div>
    );
};

export default TextArea;
