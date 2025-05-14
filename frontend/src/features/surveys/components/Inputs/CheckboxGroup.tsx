import React from 'react';
import { useDispatch, useSelector } from 'react-redux';
import { setCheckboxValue } from '../../slices/surveySlice';
import {RootState} from "../../../../app/store";
import {MultipleChoiceDto} from "../../../../shared/dto/DesignedSurveyDto";

interface CheckboxGroupProps {
    name: string;
    options: MultipleChoiceDto[];
    label?: string;
}

const CheckboxGroup: React.FC<CheckboxGroupProps> = ({ name, options, label }) => {
    const dispatch = useDispatch();
    const selectedValues = useSelector((state: RootState) => state.surveyForm[name] || []);

    const handleChange = (value: string) => {
        dispatch(setCheckboxValue({ name, value }));
    };

    return (
        <div className="flex flex-col gap-2">
            {label && <label className="text-white">{label}</label>}
            {options.map((option) => (
                <label key={option.MultipleChoiceId} className="flex items-center gap-2 text-white cursor-pointer">
                    <input
                        type="checkbox"
                        checked={selectedValues.includes(option.MultipleChoiceId.toString())}
                        onChange={() => handleChange(option.MultipleChoiceId.toString())}
                        className="w-4 h-4 border-2 border-white bg-transparent appearance-none checked:bg-white checked:border-white focus:outline-none cursor-pointer"
                    />
                    {option.MultipleChoiceName}
                </label>
            ))}
        </div>
    );
};

export default CheckboxGroup;
