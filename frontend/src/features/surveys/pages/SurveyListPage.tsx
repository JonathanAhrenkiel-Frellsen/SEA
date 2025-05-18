import { ArrowRight } from 'lucide-react';
import { Button } from '../../../shared/components/Buttons/Button';
import React, { useEffect, useState } from 'react';
import NewSurveyModal from '../components/Modals/NewSurveyModal/NewSurveyModal';
import { fetchSurveys, pauseSurvey, resumeSurvey } from '../api/surveyApi';
import { DesignedSurveyDto } from '../../../shared/dto/DesignedSurveyDto';
import { formatDate } from '../../../shared/utils/formatDate';

const SurveyListPage: React.FC = () => {
    const [showModal, setShowModal] = useState(false);
    const [surveys, setSurveys] = useState<DesignedSurveyDto[]>([]);

    // Hent alle surveys inkl. pause-flag
    const loadSurveys = () => {
        fetchSurveys()
            .then(data => setSurveys(data))
            .catch(console.error);
    };

    // Kør load én gang ved mount
    useEffect(() => {
        loadSurveys();
    }, []);

    return (
        <div className="bg-main text-white font-josefin flex flex-col gap-4 relative">
            {surveys.map(survey => (
                <div
                    key={survey.SurveyId}
                    className="border border-white p-4 flex justify-between items-center"
                >
                    <div>
                        <h2 className="text-lg font-semibold">{survey.SurveyTitle}</h2>
                        <p className="text-sm text-white/70">
                            {survey.StartDate ? formatDate(survey.StartDate.toString()) : ''}
                        </p>
                        <p className="text-sm text-white/70">{survey.ResponseCount ?? 0} responses</p>
                        <p className="text-sm">
                            Status: {survey.Published
                            ? (survey.IsPaused ? 'Paused' : 'Active')
                            : '—'}
                        </p>
                        <p className="text-sm">
                            Visibility: {survey.Published ? 'Published' : 'Draft'}
                        </p>

                    </div>

                    <div className="flex gap-2">
                        {/* Pause/Resume for publicerede surveys */}
                        {survey.Published && (
                            survey.IsPaused
                                ? <Button
                                    text="Resume"
                                    type="secondary"
                                    onClick={() => resumeSurvey(survey.SurveyId!).then(loadSurveys)}
                                />
                                : <Button
                                    text="Pause"
                                    type="secondary"
                                    onClick={() => pauseSurvey(survey.SurveyId!).then(loadSurveys)}
                                />
                        )}

                        <Button
                            text="Analysis"
                            type="secondary"
                            onClick={() => window.location.href = `/analysis/${survey.SurveyId}`}
                        />

                        <Button
                            text="Manage"
                            type="primary"
                            icon={<ArrowRight size={16} />}
                            onClick={() => window.location.href = `/surveys/${survey.SurveyId}/edit`}
                        />
                    </div>
                </div>
            ))}

            <div className="flex justify-end mt-10">
                <Button
                    text="Make new survey"
                    type="primary"
                    icon={<ArrowRight size={16} />}
                    onClick={() => setShowModal(true)}
                />
            </div>

            {showModal && <NewSurveyModal onClose={() => setShowModal(false)} />}
        </div>
    );
};

export default SurveyListPage;
