import { ArrowRight } from 'lucide-react';
import { Button } from '../components/Buttons/Button';
import {useState} from "react";
import NewSurveyModal from '../components/NewSurveyModal/NewSurveyModal';

// TODO: Replace with actual data fetching logic
const dummySurveys = [
    { id: 1, title: 'Eating habits survey', date: '2025-04-22' },
    { id: 2, title: 'Pet survey', date: '2025-04-20' },
];

const SurveyListPage = () => {
    const [showModal, setShowModal] = useState(false);

    return (
        <div className="bg-main text-white font-josefin flex flex-col gap-4 relative">
            {dummySurveys.map((survey) => (
                <div
                    key={survey.id}
                    className="border border-white p-4 flex justify-between items-center"
                >
                    <div>
                        <h2 className="text-lg font-semibold">{survey.title}</h2>
                        <p className="text-sm text-white/70">{survey.date}</p>
                    </div>
                    <div className="flex gap-2">
                        <Button text="Analysis" type="secondary" onClick={() => window.location.href = `/analysis/${survey.id}`} />

                        <Button text="Manage" type="primary" icon={<ArrowRight size={16} />} onClick={() => window.location.href = `/surveys/${survey.id}/edit`} />
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
