import { LineChart, Line, XAxis, YAxis, CartesianGrid, Tooltip, ResponsiveContainer } from 'recharts';
import {ArrowLeft, Upload, UploadIcon} from 'lucide-react';
import {Button} from "../../../shared/components/Buttons/Button";

const dummyData = [
    { day: 0, answers: 10 },
    { day: 1, answers: 50 },
    { day: 2, answers: 5 },
    { day: 3, answers: 20 },
    { day: 4, answers: 60 },
    { day: 5, answers: 100 },
    { day: 6, answers: 90 },
    { day: 7, answers: 45 },
    { day: 8, answers: 40 },
];

const SurveyAnalysisPage = () => {
    return (
        <div className="min-h-screen bg-main text-white p-6 font-josefin flex flex-col gap-10">
            <div className={'mb-6'}>
                <Button text="Go Back" icon={<ArrowLeft size={16} />} type="secondary" onClick={() => window.location.href = '/surveys'} />
            </div>

            <div className="w-full h-72 bg-secondary p-4">
                <ResponsiveContainer width="100%" height="100%">
                    <LineChart data={dummyData}>
                        <CartesianGrid stroke="#444" strokeDasharray="5 5" />
                        <XAxis
                            dataKey="day"
                            stroke="#fff"
                            label={{ value: "Day count", position: "insideBottom", dy: 10, fill: "#fff" }}
                        />
                        <YAxis
                            stroke="#fff"
                            label={{ value: "Number of Answers", angle: -90, position: "insideLeft", dx: -10, fill: "#fff" }}
                        />
                        <Tooltip contentStyle={{ backgroundColor: '#1f2a33', borderColor: '#ccc' }} />
                        <Line type="monotone" dataKey="answers" stroke="#8884d8" strokeWidth={2} />
                    </LineChart>

                </ResponsiveContainer>
            </div>

            <div className="bg-white text-main text-center py-6 font-semibold">
                Missing more graphs...
            </div>

            <div className="flex justify-end">
                <Button text={'Export Survey'} type={'secondary'} icon={<UploadIcon size={16} />} onClick={() => window.location.href = '/surveys'}/>
            </div>
        </div>
    );
};

export default SurveyAnalysisPage;
