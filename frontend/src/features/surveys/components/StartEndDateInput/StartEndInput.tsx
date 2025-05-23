import React from 'react';
import TextInput from "../../../../shared/components/Input/TextInput";
import { Controller, Control } from 'react-hook-form';
import { SurveyForm } from '../../types/SurveyForm'; // Adjust import path

interface StartEndInputProps {
  control: Control<SurveyForm>;
}

export const StartEndInput = ({ control }: StartEndInputProps) => {
  return (
    <div className="flex gap-4">
      <div className="flex flex-col gap-2 min-w-[140px]">
        <label className="text-white">Start Date</label>
        <Controller
          name="startDate"
          control={control}
          render={({ field }) => (
            <TextInput
              value={field.value ?? ''}
              setValue={field.onChange}
              type="date"
            />
          )}
        />
      </div>
      <div className="flex flex-col gap-2 min-w-[140px]">
        <label className="text-white">End Date</label>
        <Controller
          name="endDate"
          control={control}
          render={({ field }) => (
            <TextInput
              value={field.value ?? ''}
              setValue={field.onChange}
              type="date"
            />
          )}
        />
      </div>
    </div>
  );
};
