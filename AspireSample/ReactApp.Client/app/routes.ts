import {
  type RouteConfig,
  index,
  prefix,
  route,
} from "@react-router/dev/routes";

export default [
  index("routes/home.tsx"),
  ...prefix("admin", [index("./routes/admin/dashboard.tsx")]),
] satisfies RouteConfig;
