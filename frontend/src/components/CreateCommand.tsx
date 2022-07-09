import axios from "axios";
import { useState } from "react";
import useApiUrl from "../useApiUrl";
import Input from "./Input";

interface myProps {
  cancelFunction(): void;
  platformId: number;
}

export default function CreateCommand({ cancelFunction, platformId }: myProps) {
  const { commandURL } = useApiUrl();
  const [commandline, setCommandline] = useState<string>("");
  const [howTo, setHowTwo] = useState<string>("");

  const postCommandUrl = `${commandURL}${platformId}/commands`;
  console.log("url", postCommandUrl);

  const createCommand = async () => {
    try {
      await axios.post(postCommandUrl, {
        HowTo: howTo,
        CommandLine: commandline,
      });
    } catch (error) {
      console.log("Error", error);
    }
  };

  const nameAndPublishedEnterd = !!commandline && !!howTo;
  //-translate-x-1/2 -translate-y-1/2
  return (
    <div className='container w-auto h-auto p-8 absolute left-1/2 top-1/2 -translate-x-1/2 -translate-y-1/2 border-4 border-black rounded-lg z-50 bg-slate-200'>
      <form>
        <h2 className='text-2xl font-medium mb-2'>Create command</h2>
        <div className='container ml-3'>
          <Input name='How to' value={howTo} onChange={setHowTwo} required />
          <Input
            name='Commandline'
            value={commandline}
            onChange={setCommandline}
            required={true}
          />
        </div>
        <button
          className='m-2 w-20 h-8 rounded-md bg-blue-400 disabled:bg-blue-200 disabled:font-thin shadow-md hover:scale-105 active:bg-blue-200 transition-all'
          onClick={() => createCommand()}
          disabled={!nameAndPublishedEnterd}
        >
          Create
        </button>
        <button
          className='m-2 w-20 h-8 rounded-md bg-red-400 shadow-md hover:scale-105 active:bg-red-200 transition-all'
          onClick={() => cancelFunction()}
        >
          Cancel
        </button>
      </form>
    </div>
  );
}
