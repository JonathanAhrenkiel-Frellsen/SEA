import { Button } from "../Buttons/Button";

const Header = () => {
    return (
        <div className={'w-full flex justify-between items-center p-4 bg-main text-white border-b-[1px] border-gray-700'}>
            <div className={'w-full max-w-[1000px] sm:w-[90%] mx-auto'}>
                <img src={'/logo.png'} alt={'logo'} className={'h-10 float-left'}/>
                <span className={'float-right'}>
                <Button type={"primary"} text={'Sign in'} onClick={ () => {
                    window.location.href = '/login';
                }} />
                </span>
            </div>
        </div>
    )
}

export default Header;
