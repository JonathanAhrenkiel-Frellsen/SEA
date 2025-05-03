import { useState } from "react";
import { Button } from "../components/Buttons/Button";
import TextInput from "../components/Inputs/TextInput";

const LoginPage = () => {
    const [fullName, setFullName] = useState("");
    const [email, setEmail] = useState("");
    const [password, setPassword] = useState("");

    const [isLogin, setIsLogin] = useState(false);

    const handleSubmit = () => {
        console.log(isLogin ? "Login" : "Signup");
    };

    return (
        <div className="flex items-center justify-center bg-main text-text font-josefin px-4">
            <div className="bg-main text-main shadow-lg p-8 w-full max-w-md border-2 border-text">
                <h1 className="text-3xl font-bold mb-6 text-center text-text">
                    {isLogin ? "Welcome Back" : "Create an Account"}
                </h1>

                <form className="space-y-4">
                    {!isLogin && (
                        <div>
                            <TextInput value={fullName} setValue={setFullName} label="Full Name" placeholder="Jhon Doe" type="text" />
                        </div>
                    )}
                    <div>
                        <TextInput value={email} setValue={setEmail} label="Email" placeholder="jhon.doe@gmail.com" type="text" />
                    </div>
                    <div>
                        <TextInput value={password} setValue={setPassword} label="Password" placeholder="•••••••" type="password" />
                    </div>

                    <div className={'flex justify-end'}>
                    <Button
                        text={isLogin ? "Login" : "Sign Up"}
                        type="primary"
                        onClick={(e: React.MouseEvent<HTMLButtonElement>) => {
                            e.preventDefault();
                            handleSubmit();
                        }}
                    />
                    </div>
                </form>

                <p className="mt-6 text-center text-sm text-text">
                    {isLogin ? "Don't have an account?" : "Already have an account?"}{" "}
                    <button
                        onClick={() => setIsLogin(!isLogin)}
                        className="text-text font-semibold underline"
                    >
                        {isLogin ? "Sign up" : "Log in"}
                    </button>
                </p>
            </div>
        </div>
    );
};

export default LoginPage;
