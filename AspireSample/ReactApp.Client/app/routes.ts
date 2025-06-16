import {
  type RouteConfig,
  index,
  layout,
  prefix,
  route,
} from "@react-router/dev/routes";

export default [
  index("routes/home.tsx"),
  ...prefix("admin", [
    layout("routes/admin/layout.tsx", [
      index("routes/admin/dashboard.tsx"),
      route("test", "routes/admin/test.tsx"),
    ]),
  ]),
] satisfies RouteConfig;
