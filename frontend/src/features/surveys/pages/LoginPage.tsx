import { useState } from "react";
import { Button } from "../components/Buttons/Button";
import TextInput from "../components/Inputs/TextInput";
import { login, register } from "../../auth/api/authApi";
import { UserDto } from "../../../shared/dto/UserDto";
import { RegisterUserDto } from "../../../shared/dto/RegisterUserDto";
import { LoginDto } from "../../../shared/dto/LoginDto";
import { useNavigate } from 'react-router-dom';

const LoginPage = () => {
    const navigate = useNavigate();

    const [fullName, setFullName] = useState("");
    const [email, setEmail] = useState("");
    const [password, setPassword] = useState("");

    const [error, setError] = useState("");

    const [isLogin, setIsLogin] = useState(false);

    const handleSuccessResponse = (user: UserDto) => {
        if (user.UserTypeId !== 3) {
            navigate('/surveys'); // This is a superuser or experimenter
        } else {
            navigate('/'); // This is an experimentee
            setIsLogin(true);
            setError('');
        }
    };

    const handleSubmit = () => {
        if (isLogin) {
            const user: LoginDto = {
                UserEmail: email,
                Password: password
            };

            login(user).then((user: UserDto) => {
                handleSuccessResponse(user);
            }).catch(err => {
                setError(err.response?.data?.message ?? "Login failed.");
            });
        } else {
            const user: RegisterUserDto = {
                UserEmail: email,
                UserPassword: password,
                UserTypeId: 3,
                UserName: fullName
            };

            register(user).then((user: UserDto) => {
                handleSuccessResponse(user);
            }).catch(err => {
                setError(err.response?.data?.message ?? "Registration failed.");
            });
        }
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
                        <TextInput
                          value={fullName}
                          setValue={setFullName}
                          label="Full Name"
                          placeholder="Jhon Doe"
                          type="text"
                        />
                    </div>
                  )}
                  <div>
                      <TextInput
                        value={email}
                        setValue={setEmail}
                        label="Email"
                        placeholder="jhon.doe@gmail.com"
                        type="email"
                      />
                  </div>
                  <div>
                      <TextInput
                        value={password}
                        setValue={setPassword}
                        label="Password"
                        placeholder="•••••••"
                        type="password"
                      />
                  </div>

                  <div className="flex justify-end">
                      <Button
                        text={isLogin ? "Login" : "Sign Up"}
                        type="primary"
                        onClick={(e: React.MouseEvent<HTMLButtonElement>) => {
                            e.preventDefault();
                            handleSubmit();
                        }}
                      />
                  </div>

                  {error && (
                    <p className="text-red-500 text-center mt-4">
                        {error}
                    </p>
                  )}
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
