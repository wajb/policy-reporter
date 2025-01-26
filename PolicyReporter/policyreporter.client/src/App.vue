<script setup lang="ts">
  import PolicyList from './components/PolicyList.vue';
</script>

<template>
  <header>
    <h1>Policy Report</h1>
  </header>
  <main>
    <form @submit.prevent="">
      <div class="filter-options">
        <div class="filter-option">
          <label for="broker">Broker</label>
          <input id="broker" list="broker-list" placeholder="&lt;All brokers&gt;" v-model="brokerFilter" />
          <datalist id="broker-list">
            <option v-for="broker in brokers" :value="broker.id" :key="broker.id">
              {{ broker.name }}
            </option>
          </datalist>
        </div>
        <div class="filter-option">
          <label for="active-only">Active policies only?</label>
          <input id="active-only" type="checkbox" v-model="activeOnlyFilter" />
        </div>
      </div>
      <button @click="$refs.policyList?.fetchData()">Get Report</button>
    </form>
    <PolicyList ref="policyList"
      :broker-filter="brokerFilter"
      :active-only-filter="activeOnlyFilter"/>
  </main>
</template>

<script lang="ts">
  type BrokerOption = {
    id: number,
    name: string,
  };

  type Data = {
    brokerFilter: string | null,
    activeOnlyFilter: boolean,
  };

  const brokers: BrokerOption[] = [
      { id: 1, name: 'broker1' },
      { id: 2, name: 'broker2' },
  ];

  export default {
    data() : Data {
      return {
        brokerFilter: null,
        activeOnlyFilter: false,
      };
    }
  };
</script>

<style scoped>
form {
  display: flex;
  align-items: start;
  justify-content: space-between;
  border: 2px solid #b4b6b5;
  border-radius: 3px;
  padding: 1em;
  margin: 1em 0;

  input, button {
    height: 1.8em;
  }
  .filter-options {
    display: flex;
    flex-direction: column;

    .filter-option {
      flex: 1;
      display: flex;

      label {
        display: block;
        width: 12em;
      }

      input:not[type='checkbox'] {
        flex: 1;
      }

    }

  }

}
</style>
