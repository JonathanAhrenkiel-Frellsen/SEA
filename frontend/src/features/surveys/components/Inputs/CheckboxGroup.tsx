import React from 'react';
import { useDispatch, useSelector } from 'react-redux';
import { setCheckboxValue } from '../../slices/surveySlice';
import {RootState} from "../../../../app/store";

interface CheckboxGroupProps {
    name: string;
    options: string[];
}

const CheckboxGroup: React.FC<CheckboxGroupProps> = ({ name, options }) => {
    const dispatch = useDispatch();
    const selectedValues = useSelector((state: RootState) => state.surveyForm[name] || []);

    const handleChange = (value: string) => {
        dispatch(setCheckboxValue({ name, value }));
    };

    return (
        <div className="flex flex-col gap-2">
            {options.map((option) => (
                <label key={option} className="flex items-center gap-2 text-white cursor-pointer">
                    <input
                        type="checkbox"
                        checked={selectedValues.includes(option)}
                        onChange={() => handleChange(option)}
                        className="w-4 h-4 border-2 border-white bg-transparent appearance-none checked:bg-white checked:border-white focus:outline-none"
                    />
                    {option}
                </label>
            ))}
        </div>
    );
};

export default CheckboxGroup;
