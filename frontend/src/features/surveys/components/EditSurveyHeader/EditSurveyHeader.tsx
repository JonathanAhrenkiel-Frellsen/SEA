import { UseFormRegister, UseFormWatch, UseFormSetValue } from 'react-hook-form';
import {SurveyForm} from "../../types/SurveyForm";

interface SurveyHeaderProps {
    register: UseFormRegister<SurveyForm>;
    watch: UseFormWatch<SurveyForm>;
    setValue: UseFormSetValue<SurveyForm>;
}

const EditSurveyHeader = ({ register, watch, setValue }: SurveyHeaderProps) => (
    <>
        <input
            {...register('title')}
            placeholder="Survey title"
            className="bg-transparent border border-white p-2 text-lg font-semibold w-full mb-6 outline-none"
        />
        <span className="flex items-center gap-2 text-white cursor-pointer">
      <input
          type="checkbox"
          checked={watch('isPrivate')}
          onChange={(e) => setValue('isPrivate', e.target.checked)}
          className="w-4 h-4 border-2 border-white bg-transparent appearance-none checked:bg-white checked:border-white focus:outline-none cursor-pointer"
      />
      Is private
    </span>
    </>
);

export default EditSurveyHeader;
