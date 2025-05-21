import React from 'react';
import { UseFormRegister, UseFormWatch, UseFormSetValue } from 'react-hook-form';
import { SurveyForm } from "../../types/SurveyForm";

interface SurveyHeaderProps {
    register: UseFormRegister<SurveyForm>;
    watch: UseFormWatch<SurveyForm>;
    setValue: UseFormSetValue<SurveyForm>;
    readOnly: boolean;
    surveyLink?: string;
    onCopy?: () => void;
    copied?: boolean;
}

const EditSurveyHeader = ({
    register,
    watch,
    setValue,
    readOnly,
    surveyLink,
    onCopy,
    copied
}: SurveyHeaderProps) => {
    // Get the current title and privacy status
    const title = watch('title');
    const isPrivate = watch('isPrivate');

    // Compose the display value for the input when readOnly
    const displayTitle = readOnly
        ? `${title || ''} (${isPrivate ? 'Private' : 'Public'})`
        : undefined;

    return (
        <>
            <div className="mb-6">
                <div className="flex flex-col">
                    <input
                        {...register('title')}
                        placeholder="Survey title"
                        className="bg-transparent border border-white p-2 text-lg font-semibold outline-none rounded-t"
                        disabled={readOnly}
                        value={displayTitle !== undefined ? displayTitle : title}
                        readOnly={readOnly}
                    />
                    {readOnly && surveyLink && (
                        <div className="flex items-center border-x border-b border-white rounded-b bg-black px-2 py-1">
                            <span className="text-sm text-white flex-1 truncate select-all">
                                <b>Link:</b> {surveyLink}
                            </span>
                            <button
                                type="button"
                                onClick={onCopy}
                                className="ml-2 bg-green-800 hover:bg-green-600 text-white px-2 py-1 rounded text-sm transition"
                            >
                                {copied ? "Copied!" : "Copy"}
                            </button>
                        </div>
                    )}
                </div>
            </div>
            {/* Only show the checkbox if the survey is being created (not readOnly) */}
            {!readOnly && (
                <span className="flex items-center gap-2 text-white cursor-pointer">
                    <input
                        type="checkbox"
                        checked={isPrivate}
                        onChange={(e) => setValue('isPrivate', e.target.checked)}
                        className="w-4 h-4 border-2 border-white bg-transparent appearance-none checked:bg-white checked:border-white focus:outline-none cursor-pointer"
                        disabled={readOnly}
                    />
                    Is private
                </span>
            )}
        </>
    );
};

export default EditSurveyHeader;
