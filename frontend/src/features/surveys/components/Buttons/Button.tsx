import React from 'react';
import classNames from 'classnames';

export type buttonType = 'primary' | 'secondary' | 'delete';

interface ButtonProps {
    text: string;
    type: buttonType;
    icon?: React.ReactNode;
    onClick: (e: React.MouseEvent<HTMLButtonElement>) => void;
    disabled?: boolean;
    loading?: boolean;
}

export const Button: React.FC<ButtonProps> = ({
                                                  text,
                                                  type,
                                                  icon,
                                                  onClick,
                                                  disabled = false,
                                                  loading = false,
                                              }) => {
    const isDisabled = disabled || loading;

    const baseStyles =
        'border font-semibold py-2 px-8 transition-all duration-300 flex items-center justify-center gap-2';

    const typeStyles = {
        primary: 'bg-white text-black border-none',
        secondary:
            'bg-transparent text-white border border-white hover:bg-white hover:text-black',
        delete:
            'bg-transparent text-white border border-red-500 hover:bg-red-500 hover:text-white',
    };

    const disabledStyles = 'opacity-50 cursor-not-allowed';

    const combinedClass = classNames(
        baseStyles,
        typeStyles[type],
        { [disabledStyles]: isDisabled }
    );

    return (
        <button
            onClick={onClick}
            className={combinedClass}
            disabled={isDisabled}
        >
            {loading && (
                <span className="animate-spin rounded-full h-4 w-4 border-2 border-white border-t-transparent"></span>
            )}
            <span>{text}</span>
            {!loading && icon && <span>{icon}</span>}
        </button>
    );
};
