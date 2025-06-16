import { Form } from "react-router";
import useInput from "~/hooks/useInput";
import logoDark from "./logo-dark.svg";
import logoLight from "./logo-light.svg";



export function Welcome() {

  const testInput = useInput<string>(
    "Test Input", 
    [
      {
        validate: (value) => value.length > 0,
        errorMessage: (value) => `Input cannot be empty, you entered: ${value}`,
      },
      {
        validate: (value) => (value === "Test Input"),
        errorMessage: (_) => `Input must match the example, you entered: "Test Input"`,
      }
    ]);

  return (
    <main className="flex items-center justify-center pt-16 pb-4">
      <div className="flex-1 flex flex-col items-center gap-16 min-h-0">
        <header className="flex flex-col items-center gap-9">
          <div className="w-[500px] max-w-[100vw] p-4">
            <img
              src={logoLight}
              alt="React Router"
              className="block w-full dark:hidden"
            />
            <img
              src={logoDark}
              alt="React Router"
              className="hidden w-full dark:block"
            />

            <Form method="POST">
              <input type="text" value={testInput.value} onChange={(e) => testInput.change(e.target.value)} />
              {!testInput.isValid && (
                <div className="text-red-500">
                  {testInput.errors.map((error, index) => (
                    <small key={index}>{error}</small>
                  ))}
                </div>
              )}

              <button disabled={
                !testInput.isValid
              }>
                Submit
              </button>
            </Form>
          </div>
        </header>
      </div>
    </main>
  );
}