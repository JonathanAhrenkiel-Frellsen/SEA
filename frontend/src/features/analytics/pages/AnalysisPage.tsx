import { useParams } from "react-router-dom";
import { useEffect, useState } from "react";
import { getSurvey } from "../../surveys/api/surveyApi";
import { DesignedSurveyDto } from "../../../shared/dto/DesignedSurveyDto";
import { Button } from "../../../shared/components/Buttons/Button";
import { ArrowLeft, UploadIcon } from "lucide-react";
import CompletionsOverTimeChart from "../components/CompletionsOverTimeChart/CompletionsOverTimeChart";
import CompletionHistogramChart from "../components/CompletionHistogramChart/CompletionHistogramChart";
import AnswersByUserTable from "../components/AnswersByUserTable/AnswersByUserTable";
import { exportSurveyCsv } from '../../surveys/api/surveyApi';


const SurveyAnalysisPage = () => {
  const { id } = useParams<{ id?: string }>();
  const [survey, setSurvey] = useState<DesignedSurveyDto | null>(null);

  useEffect(() => {
    if (!id) return;

    getSurvey(Number(id))
      .then(setSurvey)
      .catch(console.error);
  }, [id]);

  if (!id) return <p className="text-white p-4">Invalid survey ID.</p>;
  if (!survey) return <p className="text-white p-4">Loading survey...</p>;
  
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

      {/* ✅ Only show button if survey is fetched */}
      {survey && (
        <div className="flex justify-end mt-6">
          <Button
            text="Export Responses"
            type="secondary"
            icon={<UploadIcon size={16} />}
            onClick={() => exportSurveyCsv(Number(id), 'completed')}
          />
        </div>
      )}
    </div>
  );
};
export default SurveyAnalysisPage;