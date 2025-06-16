import { reactRouter } from "@react-router/dev/vite";
import tailwindcss from "@tailwindcss/vite";
import { defineConfig } from "vite";
import tsconfigPaths from "vite-tsconfig-paths";
import { fileURLToPath, URL } from "node:url";
import fs from "fs";
import path from "path";
import child_process from "child_process";
import { env } from "process";

// const baseFolder: string =
//   env.APPDATA !== undefined && env.APPDATA !== ""
//     ? `${env.APPDATA}/ASP.NET/https`
//     : `${env.HOME}/.aspnet/https`;

// const certificateName = "reactapp.client";
// const certFilePath = path.join(baseFolder, `${certificateName}.pem`);
// const keyFilePath = path.join(baseFolder, `${certificateName}.key`);

// if (!fs.existsSync(baseFolder)) {
//   fs.mkdirSync(baseFolder, { recursive: true });
// }

// if (!fs.existsSync(certFilePath) || !fs.existsSync(keyFilePath)) {
//   if (
//     0 !==
//     child_process.spawnSync(
//       "dotnet",
//       [
//         "dev-certs",
//         "https",
//         "--export-path",
//         certFilePath,
//         "--format",
//         "Pem",
//         "--no-password",
//       ],
//       { stdio: "inherit" }
//     ).status
//   ) {
//     throw new Error("Could not create certificate.");
//   }
// }

const target: string = env.ASPNETCORE_HTTPS_PORT
  ? `https://localhost:${env.ASPNETCORE_HTTPS_PORT}`
  : env.ASPNETCORE_URLS
  ? env.ASPNETCORE_URLS.split(";")[0]
  : "https://localhost:7177";

export default defineConfig({
  plugins: [tailwindcss(), reactRouter(), tsconfigPaths()],
  server: {
    proxy: {
      "^/test": {
        target,
        secure: false,
      },
    },
    // port: parseInt(env.DEV_SERVER_PORT || "52564"),
    // https: {
    //   key: fs.readFileSync(keyFilePath),
    //   cert: fs.readFileSync(certFilePath),
    // },
  },
});
