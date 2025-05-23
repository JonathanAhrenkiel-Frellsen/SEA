import React from 'react';

interface TextInputProps {
    value: string;
    setValue: (value: string) => void;
    label?: string;
    placeholder?: string;
    type?: 'text' | 'email' | 'password' | 'number' | 'date';
}

const TextInput: React.FC<TextInputProps> = ({
                                                 value,
                                                 setValue,
                                                 label,
                                                 placeholder,
                                                 type = 'text',
                                             }) => {
    const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
        setValue(e.target.value);
    };

    return (
        <div className="flex flex-col gap-1">
            {label && <label className="text-white">{label}</label>}
            <input
                type={type}
                value={value}
                onChange={handleChange}
                placeholder={placeholder}
                className="bg-transparent text-white border border-white p-2 outline-none box-border"
            />
        </div>
    );
};

export default TextInput;
