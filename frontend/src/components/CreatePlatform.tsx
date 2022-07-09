import axios from "axios";
import { useState } from "react";
import useApiUrl from "../useApiUrl";
import Input from "./Input";

interface myProps {
  cancelFunction(): void;
}

export default function CreatePlatform({ cancelFunction }: myProps) {
  const { platformURL } = useApiUrl();
  const [name, setName] = useState<string>("");
  const [publisher, setPublisher] = useState<string>("");
  const [cost, setCost] = useState<string>("");

  const createPlatform = async () => {
    await axios.post(platformURL, { name, publisher, cost });
  };

  const nameAndPublishedEnterd = !!name && !!publisher;
  //-translate-x-1/2 -translate-y-1/2
  return (
    <div className='container w-auto h-auto p-8 absolute left-1/2 top-1/2 -translate-x-1/2 -translate-y-1/2 border-4 border-black rounded-lg z-50 bg-slate-200'>
      <form>
        <h2 className='text-2xl font-medium mb-2'>Create platform</h2>
        <div className='container ml-3'>
          <Input name='Name' value={name} onChange={setName} required={true} />
          <Input
            name='Company'
            value={publisher}
            onChange={setPublisher}
            required
          />
          <Input name='Cost' value={cost} onChange={setCost} />
        </div>
        <button
          className='m-2 w-20 h-8 rounded-md bg-blue-400 disabled:bg-blue-200 disabled:font-thin shadow-md hover:scale-105 active:bg-blue-200 transition-all'
          onClick={() => createPlatform()}
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
