export default async function AdminLayout({
  children,
}: {
  readonly children: React.ReactNode;
}) {
  return (
    <div className="flex h-full w-full flex-col">
      <div className="flex h-16 w-full items-center justify-between border-b px-4">
        <h1 className="text-xl font-bold">Admin</h1>
      </div>
      <main className="flex-1 overflow-y-auto p-4">{children}</main>
    </div>
  );
}
