export default function Dashboard() {
  return (
    <div className="flex h-full w-full flex-col">
      <div className="flex h-16 w-full items-center justify-between border-b px-4">
        <h2 className="text-xl font-bold">Admin Dashboard</h2>
      </div>
      <main className="flex-1 overflow-y-auto p-4">
        <p>Welcome to the admin dashboard!</p>
      </main>
    </div>
  );
}