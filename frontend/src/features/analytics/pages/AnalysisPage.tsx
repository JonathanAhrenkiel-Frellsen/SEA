import { ArrowLeft, UploadIcon } from "lucide-react";
import { Button } from "../../../shared/components/Buttons/Button";
import { useParams } from "react-router-dom";
import CompletionsOverTimeChart from "../components/CompletionsOverTimeChart/CompletionsOverTimeChart";
import CompletionHistogramChart from "../components/CompletionHistogramChart/CompletionHistogramChart";
import AnswersByUserTable from "../components/AnswersByUserTable/AnswersByUserTable";

const SurveyAnalysisPage = () => {
    const { id } = useParams<{ id?: string }>();

    if (!id) return <p className="text-white p-4">Invalid survey ID.</p>;

    return (
        <div className="min-h-screen bg-main text-white p-6 font-josefin flex flex-col gap-10">
            <div className="mb-6">
                <Button
                    text="Go Back"
                    icon={<ArrowLeft size={16} />}
                    type="secondary"
                    onClick={() => window.location.href = "/surveys"}
                />
            </div>

            <CompletionsOverTimeChart surveyId={id} />
            <CompletionHistogramChart surveyId={id} />
            <AnswersByUserTable surveyId={id} />

            <div className="flex justify-end mt-6">
                <Button
                    text="Export Survey"
                    type="secondary"
                    icon={<UploadIcon size={16} />}
                    onClick={() => window.location.href = "/surveys"}
                />
            </div>
        </div>
    );
};

export default SurveyAnalysisPage;
