import {
  Outlet
} from "react-router";
import useAuth from "./useAuth";

export type RequireAuthProps = {
    roles?: string[];
}

export default function RequireAuth({roles = []}: RequireAuthProps) {

    const {isAuthenticated, hasRole } = useAuth();
    
    return isAuthenticated() && hasRole(roles) 
        ? (<Outlet/>) 
        : isAuthenticated()
            ? "Unsupported role" 
            : "Please log in";

}