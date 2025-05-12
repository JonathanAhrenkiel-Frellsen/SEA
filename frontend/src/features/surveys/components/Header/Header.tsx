import { Button } from "../../../../shared/components/Buttons/Button";
import {useDispatch, useSelector} from "react-redux";
import {selectUser} from "../../../auth/slices/authSlice";
import {useNavigate} from "react-router-dom";
import {logout} from "../../../auth/api/authApi";

const Header = () => {
  const user = useSelector(selectUser)
  const navigator = useNavigate()
  const dispatch = useDispatch()

  return (
    <div className={'w-full flex justify-between items-center p-4 bg-main text-white border-b-[1px] border-gray-700'}>
      <div className={'w-full max-w-[1000px] sm:w-[90%] mx-auto'}>
        <img src={'/logo.png'} alt={'logo'} className={'h-10 float-left'}/>
        <span className={'float-right'}>
        {!user ? <Button type={"primary"} text={'Sign in'} onClick={ () => {
          navigator('/login')
        }} /> : <>
          <Button type={"primary"} text={'Logout'} onClick={ () => {
            logout().then(() => {
              navigator('/')
            })
          }} />
        </> }
        </span>
      </div>
    </div>
  )
}

export default Header;
