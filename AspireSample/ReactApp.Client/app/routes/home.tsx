import type { Route } from "./+types/home";
import { Welcome } from "../welcome/welcome";
import { useEffect } from "react";

export function meta({}: Route.MetaArgs) {
  return [
    { title: "New React Router App" },
    { name: "description", content: "Welcome to React Router!" },
  ];
}

export default function Home() {
  useEffect(() => {
    async function ping() {
      const response = await fetch("/api/catalog");
      const text = await response.text();
      console.log(text);
    }
    ping();
  }, []);

  return <Welcome />;
}
