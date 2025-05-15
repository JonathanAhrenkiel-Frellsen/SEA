import { ArrowRight } from 'lucide-react';
import { Button } from '../../../shared/components/Buttons/Button';
import {useEffect, useState} from "react";
import NewSurveyModal from '../components/Modals/NewSurveyModal/NewSurveyModal';
import {fetchSurveys} from "../api/surveyApi";
import {DesignedSurveyDto} from "../../../shared/dto/DesignedSurveyDto";
import {formatDate} from "../../../shared/utils/formatDate";

const SurveyListPage = () => {
    const [showModal, setShowModal] = useState(false);
    const [surveys, setSurveys] = useState<DesignedSurveyDto[]>([]);

  useEffect(() => {
    fetchSurveys().then(res => {
      setSurveys(res)
    })
  }, []);

    return (
        <div className="bg-main text-white font-josefin flex flex-col gap-4 relative">
            {surveys.map((survey) => (
                <div
                    key={survey.SurveyId}
                    className="border border-white p-4 flex justify-between items-center"
                >
                    <div>
                        <h2 className="text-lg font-semibold">{survey.SurveyTitle}</h2>
                        <p className="text-sm text-white/70">{survey.StartDate ? formatDate(survey.StartDate.toString()) : ''}</p>
                        <p className="text-sm text-white/70">{survey.ResponseCount ?? 0} responses</p>
                    </div>
                    <div className="flex gap-2">
                        <Button text="Analysis" type="secondary" onClick={() => window.location.href = `/analysis/${survey.SurveyId}`} />

                        <Button text="Manage" type="primary" icon={<ArrowRight size={16} />} onClick={() => window.location.href = `/surveys/${survey.SurveyId}/edit`} />
                    </div>
                </div>
            ))}

            <div className={'flex justify-end mt-10'}>
                <Button text="Make new survey" type="primary" icon={<ArrowRight size={16} />} onClick={() => setShowModal(true)} />
            </div>

            {showModal && <NewSurveyModal onClose={() => setShowModal(false)} />}
        </div>
    );
};

export default SurveyListPage;
