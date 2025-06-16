import { Outlet } from "react-router";

export default function Layout() {
    return (
        <div>
            <h1>Admin Layout</h1>
            <nav>
                <ul>
                    <li><a href="/admin/dashboard">Dashboard</a></li>
                    <li><a href="/admin/test">Test</a></li>
                    <li><a href="/admin/test2">Test</a></li>
                </ul>
            </nav>
            <main><Outlet/></main>
        </div>
    );

}